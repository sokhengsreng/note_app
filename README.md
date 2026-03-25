# Notes Application

A full-stack notes app built with:

- Frontend: Vue 3 + TypeScript + Tailwind CSS
- Backend: ASP.NET Core Web API (C#)
- Database: SQL Server
- Data access: Dapper

This project supports note CRUD, search, sorting/filtering, auth, and per-user note ownership.

## What is implemented

### Core note flow

- Create note with required `title`, optional `content`
- Auto-set `createdAt` and `updatedAt`
- Read list + note detail page
- Update note (title/content)
- Delete note from active list (soft-delete to trash)

### Frontend

- Login/Register pages
- Notes list page with CRUD actions
- Search
- Sorting/filtering (desktop and mobile controls)
- Responsive UI
- Axios API integration
- Pinia state management

### Backend

- Auth + JWT-protected endpoints
- Notes CRUD API
- User can only read/update/delete their own notes
- SQL Server + Dapper repositories

## Project structure

- `NotesApplication/` - backend API
- `NotesApplication.Frontend/` - Vue frontend
- `NotesApplication.Tests/` - unit tests
- `docker-compose.yml` - optional containerized setup

## Quick start

For fastest setup, read `QUICKSTART.md`.

## Local development

### 1) Backend

```bash
cd NotesApplication
dotnet restore
dotnet run
```

By default (current launch settings), the API runs on:

- `http://localhost:5207`

In development, built-in OpenAPI is available via:

- `/openapi/v1.json`

### 2) Frontend

```bash
cd NotesApplication.Frontend
npm install
npm run dev
```

Frontend dev server usually runs on:

- `http://localhost:5173`

Frontend API target should be:

- `VITE_API_URL=/api` (default in `.env.example`)

### 3) Tests

```bash
cd NotesApplication.Tests
dotnet test
```

## Notes for reviewers

- The app uses soft-delete (`IsDeleted`) for trash behavior.
- Some older markdown files were kept only as archived pointers; use this README + QUICKSTART as source of truth.
