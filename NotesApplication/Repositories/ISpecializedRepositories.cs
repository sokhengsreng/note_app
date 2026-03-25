using NotesApplication.Models;

namespace NotesApplication.Repositories;

public interface INoteRepository : IRepository<Note>
{
    Task<IEnumerable<Note>> GetUserNotesAsync(int userId);
    Task<Note?> GetNoteByIdForUserAsync(int noteId, int userId);
    Task<(IEnumerable<Note> notes, int totalCount)> GetUserNotesWithPaginationAsync(
        int userId, int pageNumber, int pageSize, string? searchTerm = null, 
        string sortBy = "CreatedAt", bool sortDescending = true, 
        bool? isFavorite = null, bool? isDeleted = null);
    Task<bool> UserOwnsNoteAsync(int noteId, int userId);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
}
