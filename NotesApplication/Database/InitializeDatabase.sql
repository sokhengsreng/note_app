-- Create NotesDb Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'NotesDb')
BEGIN
    CREATE DATABASE NotesDb;
END
GO

USE NotesDb;
GO

-- Create Users Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE [dbo].[Users]
    (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Username] NVARCHAR(100) NOT NULL UNIQUE,
        [Email] NVARCHAR(255) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL
    );
    
    CREATE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
    CREATE INDEX [IX_Users_Username] ON [dbo].[Users] ([Username]);
END
GO

-- Create Notes Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notes')
BEGIN
    CREATE TABLE [dbo].[Notes]
    (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [Title] NVARCHAR(255) NOT NULL,
        [Content] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [IsFavorite] BIT NOT NULL DEFAULT 0,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_Notes_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_Notes_UserId] ON [dbo].[Notes] ([UserId]);
    CREATE INDEX [IX_Notes_CreatedAt] ON [dbo].[Notes] ([CreatedAt]);
    CREATE INDEX [IX_Notes_Title] ON [dbo].[Notes] ([Title]);
END
GO

-- Create AuditLog Table for future AI features and tracking
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLogs')
BEGIN
    CREATE TABLE [dbo].[AuditLogs]
    (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [Action] NVARCHAR(100) NOT NULL,
        [EntityType] NVARCHAR(100) NOT NULL,
        [EntityId] INT NOT NULL,
        [OldValue] NVARCHAR(MAX) NULL,
        [NewValue] NVARCHAR(MAX) NULL,
        [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_AuditLogs_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs] ([UserId]);
    CREATE INDEX [IX_AuditLogs_Timestamp] ON [dbo].[AuditLogs] ([Timestamp]);
END
GO

-- Create NoteAnalytics Table for future AI features (sentiment analysis, categorization, etc.)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'NoteAnalytics')
BEGIN
    CREATE TABLE [dbo].[NoteAnalytics]
    (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [NoteId] INT NOT NULL,
        [Sentiment] NVARCHAR(50) NULL,
        [Category] NVARCHAR(100) NULL,
        [Keywords] NVARCHAR(MAX) NULL,
        [Summary] NVARCHAR(MAX) NULL,
        [AnalyzedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_NoteAnalytics_Notes] FOREIGN KEY ([NoteId]) REFERENCES [dbo].[Notes]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_NoteAnalytics_NoteId] ON [dbo].[NoteAnalytics] ([NoteId]);
    CREATE INDEX [IX_NoteAnalytics_Sentiment] ON [dbo].[NoteAnalytics] ([Sentiment]);
    CREATE INDEX [IX_NoteAnalytics_Category] ON [dbo].[NoteAnalytics] ([Category]);
END
GO

PRINT 'Database schema created successfully!';
