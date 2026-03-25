using NotesApplication.Data;
using NotesApplication.Models;
using Dapper;

namespace NotesApplication.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = "SELECT * FROM Users WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = "SELECT * FROM Users";
        return await connection.QueryAsync<User>(query);
    }

    public async Task<int> AddAsync(User entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = @"
            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt, UpdatedAt)
            VALUES (@Username, @Email, @PasswordHash, @CreatedAt, @UpdatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        
        return await connection.QuerySingleAsync<int>(query, entity);
    }

    public async Task<bool> UpdateAsync(User entity)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = @"
            UPDATE Users 
            SET Username = @Username, Email = @Email, PasswordHash = @PasswordHash, UpdatedAt = @UpdatedAt
            WHERE Id = @Id";
        
        var affectedRows = await connection.ExecuteAsync(query, entity);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = "DELETE FROM Users WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(query, new { Id = id });
        return affectedRows > 0;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = "SELECT * FROM Users WHERE Email = @Email";
        return await connection.QueryFirstOrDefaultAsync<User>(query, new { Email = email });
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();
        const string query = "SELECT * FROM Users WHERE Username = @Username";
        return await connection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });
    }
}
