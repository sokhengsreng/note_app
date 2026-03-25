using Moq;
using NotesApplication.DTOs;
using NotesApplication.Models;
using NotesApplication.Repositories;
using NotesApplication.Services;
using Xunit;

namespace NotesApplication.Tests.Services;

public class NoteServiceTests
{
    private readonly Mock<INoteRepository> _mockNoteRepository;
    private readonly NoteService _noteService;

    public NoteServiceTests()
    {
        _mockNoteRepository = new Mock<INoteRepository>();
        _noteService = new NoteService(_mockNoteRepository.Object);
    }

    [Fact]
    public async Task CreateNoteAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var userId = 1;
        var createDto = new CreateNoteDto { Title = "Test Note", Content = "Test Content" };
        var expectedNoteId = 1;

        _mockNoteRepository.Setup(r => r.AddAsync(It.IsAny<Note>()))
            .ReturnsAsync(expectedNoteId);

        // Act
        var result = await _noteService.CreateNoteAsync(userId, createDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Note created successfully", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("Test Note", result.Data.Title);
        Assert.Equal("Test Content", result.Data.Content);
    }

    [Fact]
    public async Task CreateNoteAsync_WithoutTitle_ShouldReturnError()
    {
        // Arrange
        var userId = 1;
        var createDto = new CreateNoteDto { Title = "", Content = "Test Content" };

        // Act
        var result = await _noteService.CreateNoteAsync(userId, createDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Title is required", result.Message);
        Assert.Contains("Title is required", result.Errors);
    }

    [Fact]
    public async Task GetNoteAsync_WithValidId_ShouldReturnNote()
    {
        // Arrange
        var userId = 1;
        var noteId = 1;
        var note = new Note { Id = noteId, UserId = userId, Title = "Test", Content = "Content", CreatedAt = DateTime.UtcNow };

        _mockNoteRepository.Setup(r => r.GetNoteByIdForUserAsync(noteId, userId))
            .ReturnsAsync(note);

        // Act
        var result = await _noteService.GetNoteAsync(userId, noteId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Test", result.Data.Title);
    }

    [Fact]
    public async Task GetNoteAsync_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var userId = 1;
        var noteId = 999;

        _mockNoteRepository.Setup(r => r.GetNoteByIdForUserAsync(noteId, userId))
            .ReturnsAsync((Note?)null);

        // Act
        var result = await _noteService.GetNoteAsync(userId, noteId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Note not found", result.Message);
    }

    [Fact]
    public async Task UpdateNoteAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var userId = 1;
        var noteId = 1;
        var updateDto = new UpdateNoteDto { Title = "Updated Title", Content = "Updated Content" };
        var existingNote = new Note { Id = noteId, UserId = userId, Title = "Old", Content = "Old", CreatedAt = DateTime.UtcNow };

        _mockNoteRepository.Setup(r => r.GetNoteByIdForUserAsync(noteId, userId))
            .ReturnsAsync(existingNote);
        _mockNoteRepository.Setup(r => r.UpdateAsync(It.IsAny<Note>()))
            .ReturnsAsync(true);

        // Act
        var result = await _noteService.UpdateNoteAsync(userId, noteId, updateDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Note updated successfully", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal("Updated Title", result.Data.Title);
    }

    [Fact]
    public async Task DeleteNoteAsync_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var userId = 1;
        var noteId = 1;
        var existingNote = new Note { Id = noteId, UserId = userId, Title = "Old", Content = "Old", CreatedAt = DateTime.UtcNow };

        _mockNoteRepository.Setup(r => r.GetNoteByIdForUserAsync(noteId, userId))
            .ReturnsAsync(existingNote);
        _mockNoteRepository.Setup(r => r.UpdateAsync(It.Is<Note>(n => n.Id == noteId && n.UserId == userId && n.IsDeleted)))
            .ReturnsAsync(true);

        // Act
        var result = await _noteService.DeleteNoteAsync(userId, noteId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Note moved to trash", result.Message);
        Assert.True(result.Data);
        _mockNoteRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteNoteAsync_WhenAlreadyInTrash_ShouldPermanentlyDelete()
    {
        var userId = 1;
        var noteId = 1;
        var trashedNote = new Note
        {
            Id = noteId,
            UserId = userId,
            Title = "Old",
            Content = "Old",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = true
        };

        _mockNoteRepository.Setup(r => r.GetNoteByIdForUserAsync(noteId, userId))
            .ReturnsAsync(trashedNote);
        _mockNoteRepository.Setup(r => r.DeleteAsync(noteId))
            .ReturnsAsync(true);

        var result = await _noteService.DeleteNoteAsync(userId, noteId);

        Assert.True(result.Success);
        Assert.Equal("Note permanently deleted", result.Message);
        Assert.True(result.Data);
        _mockNoteRepository.Verify(r => r.DeleteAsync(noteId), Times.Once);
        _mockNoteRepository.Verify(r => r.UpdateAsync(It.IsAny<Note>()), Times.Never);
    }

    [Fact]
    public async Task DeleteNoteAsync_WithoutPermission_ShouldReturnError()
    {
        // Arrange
        var userId = 1;
        var noteId = 1;

        _mockNoteRepository.Setup(r => r.UserOwnsNoteAsync(noteId, userId))
            .ReturnsAsync(false);

        // Act
        var result = await _noteService.DeleteNoteAsync(userId, noteId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Note not found", result.Message);
    }

    [Fact]
    public async Task EmptyTrashAsync_ShouldDeleteAllTrashedAndReturnCount()
    {
        var userId = 1;
        _mockNoteRepository.Setup(r => r.DeleteAllTrashedForUserAsync(userId))
            .ReturnsAsync(3);

        var result = await _noteService.EmptyTrashAsync(userId);

        Assert.True(result.Success);
        Assert.Equal(3, result.Data);
        Assert.Contains("3", result.Message);
        _mockNoteRepository.Verify(r => r.DeleteAllTrashedForUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserNotesAsync_WithValidQuery_ShouldReturnPaginatedNotes()
    {
        // Arrange
        var userId = 1;
        var query = new GetNotesQueryDto { PageNumber = 1, PageSize = 10 };
        var notes = new List<Note>
        {
            new() { Id = 1, UserId = userId, Title = "Note 1", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, UserId = userId, Title = "Note 2", CreatedAt = DateTime.UtcNow }
        };

        _mockNoteRepository.Setup(r => r.GetUserNotesWithPaginationAsync(userId, 1, 10, null, "CreatedAt", true))
            .ReturnsAsync((notes, 2));

        // Act
        var result = await _noteService.GetUserNotesAsync(userId, query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Items.Count);
        Assert.Equal(2, result.Data.TotalCount);
    }
}
