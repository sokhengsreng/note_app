using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApplication.DTOs;
using NotesApplication.Services;
using System.Security.Claims;

namespace NotesApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }

    /// <summary>
    /// Get paginated list of user's notes
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="searchTerm">Search term to filter notes</param>
    /// <param name="sortBy">Field to sort by (CreatedAt, Title, UpdatedAt)</param>
    /// <param name="sortDescending">Sort in descending order</param>
    /// <returns>Paginated list of notes</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<NoteResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNotes(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = true,
        [FromQuery] bool? isFavorite = null,
        [FromQuery] bool? isDeleted = null)
    {
        var userId = GetUserId();
        var query = new GetNotesQueryDto
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortDescending = sortDescending,
            IsFavorite = isFavorite,
            IsDeleted = isDeleted
        };

        var result = await _noteService.GetUserNotesAsync(userId, query);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific note by ID
    /// </summary>
    /// <param name="id">Note ID</param>
    /// <returns>Note details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNote(int id)
    {
        var userId = GetUserId();
        var result = await _noteService.GetNoteAsync(userId, id);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new note
    /// </summary>
    /// <param name="dto">Note creation details</param>
    /// <returns>Created note</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteDto dto)
    {
        var userId = GetUserId();
        var result = await _noteService.CreateNoteAsync(userId, dto);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetNote), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Update an existing note
    /// </summary>
    /// <param name="id">Note ID</param>
    /// <param name="dto">Updated note details</param>
    /// <returns>Updated note</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<NoteResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteDto dto)
    {
        var userId = GetUserId();
        var result = await _noteService.UpdateNoteAsync(userId, id, dto);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete a note
    /// </summary>
    /// <param name="id">Note ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteNote(int id)
    {
        var userId = GetUserId();
        var result = await _noteService.DeleteNoteAsync(userId, id);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
