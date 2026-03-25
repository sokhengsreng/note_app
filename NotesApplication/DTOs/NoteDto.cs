namespace NotesApplication.DTOs;

public class CreateNoteDto
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
}

public class UpdateNoteDto
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public bool? IsFavorite { get; set; }
    public bool? IsDeleted { get; set; }
}

public class NoteResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsFavorite { get; set; }
    public bool IsDeleted { get; set; }
}

public class GetNotesQueryDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
    public bool? IsFavorite { get; set; }
    public bool? IsDeleted { get; set; }
}
