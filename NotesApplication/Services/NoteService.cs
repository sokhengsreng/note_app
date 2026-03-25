using NotesApplication.DTOs;
using NotesApplication.Models;
using NotesApplication.Repositories;

namespace NotesApplication.Services;

public interface INoteService
{
    Task<ApiResponse<NoteResponseDto>> CreateNoteAsync(int userId, CreateNoteDto dto);
    Task<ApiResponse<NoteResponseDto>> GetNoteAsync(int userId, int noteId);
    Task<ApiResponse<PaginatedResponse<NoteResponseDto>>> GetUserNotesAsync(int userId, GetNotesQueryDto query);
    Task<ApiResponse<NoteResponseDto>> UpdateNoteAsync(int userId, int noteId, UpdateNoteDto dto);
    Task<ApiResponse<bool>> DeleteNoteAsync(int userId, int noteId);
}

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;

    public NoteService(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public async Task<ApiResponse<NoteResponseDto>> CreateNoteAsync(int userId, CreateNoteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return new ApiResponse<NoteResponseDto>
            {
                Success = false,
                Message = "Title is required",
                Errors = new List<string> { "Title is required" }
            };
        }

        var note = new Note
        {
            UserId = userId,
            Title = dto.Title,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var noteId = await _noteRepository.AddAsync(note);
        note.Id = noteId;

        return new ApiResponse<NoteResponseDto>
        {
            Success = true,
            Message = "Note created successfully",
            Data = MapToDto(note)
        };
    }

    public async Task<ApiResponse<NoteResponseDto>> GetNoteAsync(int userId, int noteId)
    {
        var note = await _noteRepository.GetNoteByIdForUserAsync(noteId, userId);
        if (note == null)
        {
            return new ApiResponse<NoteResponseDto>
            {
                Success = false,
                Message = "Note not found",
                Errors = new List<string> { "Note not found or you don't have permission to access it" }
            };
        }

        return new ApiResponse<NoteResponseDto>
        {
            Success = true,
            Message = "Note retrieved successfully",
            Data = MapToDto(note)
        };
    }

    public async Task<ApiResponse<PaginatedResponse<NoteResponseDto>>> GetUserNotesAsync(int userId, GetNotesQueryDto query)
    {
        var (notes, totalCount) = await _noteRepository.GetUserNotesWithPaginationAsync(
            userId,
            query.PageNumber,
            query.PageSize,
            query.SearchTerm,
            query.SortBy,
            query.SortDescending,
            query.IsFavorite,
            query.IsDeleted
        );

        var totalPages = (totalCount + query.PageSize - 1) / query.PageSize;

        var response = new PaginatedResponse<NoteResponseDto>
        {
            Items = notes.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalPages = totalPages
        };

        return new ApiResponse<PaginatedResponse<NoteResponseDto>>
        {
            Success = true,
            Message = "Notes retrieved successfully",
            Data = response
        };
    }

    public async Task<ApiResponse<NoteResponseDto>> UpdateNoteAsync(int userId, int noteId, UpdateNoteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return new ApiResponse<NoteResponseDto>
            {
                Success = false,
                Message = "Title is required",
                Errors = new List<string> { "Title is required" }
            };
        }

        var note = await _noteRepository.GetNoteByIdForUserAsync(noteId, userId);
        if (note == null)
        {
            return new ApiResponse<NoteResponseDto>
            {
                Success = false,
                Message = "Note not found",
                Errors = new List<string> { "Note not found or you don't have permission to access it" }
            };
        }

        note.Title = dto.Title ?? note.Title;
        note.Content = dto.Content ?? note.Content;
        if (dto.IsFavorite.HasValue) note.IsFavorite = dto.IsFavorite.Value;
        if (dto.IsDeleted.HasValue) note.IsDeleted = dto.IsDeleted.Value;
        note.UpdatedAt = DateTime.UtcNow;

        await _noteRepository.UpdateAsync(note);

        return new ApiResponse<NoteResponseDto>
        {
            Success = true,
            Message = "Note updated successfully",
            Data = MapToDto(note)
        };
    }

    public async Task<ApiResponse<bool>> DeleteNoteAsync(int userId, int noteId)
    {
        var note = await _noteRepository.GetNoteByIdForUserAsync(noteId, userId);
        if (note == null)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Note not found",
                Errors = new List<string> { "Note not found or you don't have permission to delete it" }
            };
        }

        // Soft delete: set IsDeleted to true
        note.IsDeleted = true;
        note.UpdatedAt = DateTime.UtcNow;
        var result = await _noteRepository.UpdateAsync(note);

        return new ApiResponse<bool>
        {
            Success = result,
            Message = result ? "Note moved to trash" : "Failed to move note to trash",
            Data = result
        };
    }

    private static NoteResponseDto MapToDto(Note note)
    {
        return new NoteResponseDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt,
            IsFavorite = note.IsFavorite,
            IsDeleted = note.IsDeleted
        };
    }
}
