# Quickstart

Use this file if you just want to run the app quickly.

## Local run (recommended)

### 1) Database (once per machine / fresh SQL instance)

From the **repository root**:

```bash
sqlcmd -S . -E -C -i NotesApplication/Database/InitializeDatabase.sql
```

Use the same SQL Server as `ConnectionStrings:DefaultConnection` in `NotesApplication/appsettings.json`. If you use SSMS or Azure Data Studio instead, run `NotesApplication/Database/InitializeDatabase.sql` there.

### 2) Backend

```bash
cd NotesApplication
dotnet restore
dotnet run
```

### 3) Frontend

```bash
cd NotesApplication.Frontend
npm install
npm run dev
```

Then open the URL Vite prints (usually `http://localhost:5173`).

---

## Docker (optional, later)

Use `docker-compose` when you want a **container** deployment-style setup—not required for normal local dev. See **“Docker (optional)”** in `README.md` for schema setup (SQL Server does not auto-run the init script; you run `InitializeDatabase.sql` against the container after it is up).

```bash
docker-compose up -d --build
```

## Verify quickly

1. Open frontend
2. Register a user
3. Create a note
4. Edit and delete a note
5. Confirm list/search/sort works

## API docs (development)

Built-in OpenAPI JSON endpoint:

- `/openapi/v1.json`
