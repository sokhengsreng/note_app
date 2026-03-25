using NotesApplication.Data;
using NotesApplication.Models;
using Dapper;

namespace NotesApplication.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public NoteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Note?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = "SELECT * FROM Notes WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Note>(query, new { Id = id });
    }

    public async Task<IEnumerable<Note>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = "SELECT * FROM Notes";
        return await connection.QueryAsync<Note>(query);
    }

    public async Task<int> AddAsync(Note entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = @"
            INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt, IsFavorite, IsDeleted)
            VALUES (@UserId, @Title, @Content, @CreatedAt, @UpdatedAt, @IsFavorite, @IsDeleted);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        
        return await connection.QuerySingleAsync<int>(query, entity);
    }

    public async Task<bool> UpdateAsync(Note entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = @"
            UPDATE Notes 
            SET Title = @Title, Content = @Content, UpdatedAt = @UpdatedAt, 
                IsFavorite = @IsFavorite, IsDeleted = @IsDeleted
            WHERE Id = @Id";
        
        var affectedRows = await connection.ExecuteAsync(query, entity);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = "DELETE FROM Notes WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
        return affectedRows > 0;
    }

    public async Task<IEnumerable<Note>> GetUserNotesAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = "SELECT * FROM Notes WHERE UserId = @UserId AND IsDeleted = 0 ORDER BY CreatedAt DESC";
        return await connection.QueryAsync<Note>(query, new { UserId = userId });
    }

    public async Task<Note?> GetNoteByIdForUserAsync(int noteId, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = "SELECT * FROM Notes WHERE Id = @Id AND UserId = @UserId";
        return await connection.QueryFirstOrDefaultAsync<Note>(query, new { Id = noteId, UserId = userId });
    }

    public async Task<(IEnumerable<Note>, int)> GetUserNotesWithPaginationAsync(
        int userId, int pageNumber, int pageSize, string? searchTerm = null, 
        string sortBy = "CreatedAt", bool sortDescending = true, 
        bool? isFavorite = null, bool? isDeleted = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereClause = "WHERE UserId = @UserId";
        
        // Handle IsDeleted filtering (default: show non-deleted)
        if (isDeleted.HasValue)
        {
            whereClause += " AND IsDeleted = @IsDeleted";
        }
        else
        {
            whereClause += " AND IsDeleted = 0";
        }

        // Handle IsFavorite filtering
        if (isFavorite.HasValue)
        {
            whereClause += " AND IsFavorite = @IsFavorite";
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            whereClause += " AND (Title LIKE @SearchTerm OR Content LIKE @SearchTerm)";
        }

        var orderClause = sortDescending ? "DESC" : "ASC";
        var orderBy = sortBy switch
        {
            "Title" => $"ORDER BY Title {orderClause}",
            "UpdatedAt" => $"ORDER BY UpdatedAt {orderClause}",
            _ => $"ORDER BY CreatedAt {orderClause}"
        };

        var offset = (pageNumber - 1) * pageSize;

        var countQuery = $"SELECT COUNT(*) FROM Notes {whereClause}";
        var totalCount = await connection.QuerySingleAsync<int>(countQuery, 
            new { UserId = userId, SearchTerm = $"%{searchTerm}%", IsFavorite = isFavorite, IsDeleted = isDeleted ?? false });

        var dataQuery = $@"
            SELECT * FROM Notes 
            {whereClause}
            {orderBy}
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

        var notes = await connection.QueryAsync<Note>(dataQuery, 
            new { UserId = userId, SearchTerm = $"%{searchTerm}%", Offset = offset, PageSize = pageSize, IsFavorite = isFavorite, IsDeleted = isDeleted ?? false });

        return (notes, totalCount);
    }

    public async Task<bool> UserOwnsNoteAsync(int noteId, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = "SELECT COUNT(*) FROM Notes WHERE Id = @NoteId AND UserId = @UserId";
        var count = await connection.QuerySingleAsync<int>(query, new { NoteId = noteId, UserId = userId });
        return count > 0;
    }

    public async Task<int> DeleteAllTrashedForUserAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = "DELETE FROM Notes WHERE UserId = @UserId AND IsDeleted = 1";
        return await connection.ExecuteAsync(query, new { UserId = userId });
    }
}
