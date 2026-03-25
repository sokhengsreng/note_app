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
- `docker-compose.yml` - **optional** container stack for deployment-style runs (not required for local dev)

## Quick start

For fastest setup, read `QUICKSTART.md`.

## Local development

**Prerequisites**

- **.NET SDK** matching the project (see `NotesApplication/NotesApplication.csproj`, currently **net10.0**)
- **Node.js** + **npm** (for the frontend)
- **SQL Server** reachable using `ConnectionStrings:DefaultConnection` in `NotesApplication/appsettings.json` (default: Windows auth to the local instance `.`)

### 1) Database (local SQL Server) — do this first on a new machine

The API uses **Dapper**, not Entity Framework—there are **no** `dotnet ef` migrations. Create the database and tables from the SQL script:

- **Script:** `NotesApplication/Database/InitializeDatabase.sql` (creates `NotesDb` and tables such as `Users`, `Notes`)
- **Run it once** against the same server as in `appsettings.json` (SQL Server Management Studio, Azure Data Studio, or `sqlcmd`).

Until `NotesDb` and the schema exist, **auth and note endpoints will fail** even though `dotnet run` and `/health` succeed.

**Example** (repository root, Windows integrated security, local default instance, matches typical `Trusted_Connection` + `TrustServerCertificate` setups):

```bash
sqlcmd -S . -E -C -i NotesApplication/Database/InitializeDatabase.sql
```

### 2) Backend

```bash
cd NotesApplication
dotnet restore
dotnet run
```

By default (current launch settings), the API runs on:

- `http://localhost:5207`

In development, built-in OpenAPI is available via:

- `/openapi/v1.json`

### 3) Frontend

```bash
cd NotesApplication.Frontend
npm install
npm run dev
```

Frontend dev server usually runs on:

- `http://localhost:5173`

Frontend API base URL:

- `VITE_API_URL=/api` (default in `.env.example`; Vite proxies `/api` to the backend in dev)

### 4) Tests

```bash
cd NotesApplication.Tests
dotnet test
```

### Troubleshooting

- **Address already in use (5207 or 5173):** Another process is using the API or Vite port. Stop that process, or change the URL in `NotesApplication/Properties/launchSettings.json` and `NotesApplication.Frontend/vite.config.ts` (and keep the Vite proxy target aligned with where the API listens).

### Docker (optional — deployment / container trial)

**Local development is intended to use .NET + Node + SQL Server on the host**, as above. `docker-compose.yml` is optional when you want a containerized stack (e.g. closer to deployment).

The official **SQL Server on Linux** image does **not** run scripts from `docker-entrypoint-initdb.d` (that pattern is PostgreSQL-style). After the `sqlserver` container is healthy, apply the schema yourself, for example:

```bash
docker cp NotesApplication/Database/InitializeDatabase.sql notes-sqlserver:/tmp/init.sql
docker exec notes-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourSuperSecurePassword123!" -C -i /tmp/init.sql
```

(Adjust the `sqlcmd` path if your image uses `mssql-tools18` or a different layout; password must match `SA_PASSWORD` in `docker-compose.yml`.)

Then start the stack:

```bash
docker-compose up -d --build
```

### GitHub Pages (optional — static frontend demo)

Project sites are served at `https://<user>.github.io/<repo>/`, not at the domain root. The frontend must be built with that **base path**, or scripts and CSS are requested from the wrong URL (`/assets/...`), which returns HTML (404 page) and the browser reports **wrong MIME type** / **NS_ERROR_CORRUPTED_CONTENT**.

From `NotesApplication.Frontend`:

```bash
npm run build:gh-pages
```

Deploy the contents of `dist/` to the `gh-pages` branch (or GitHub Actions “Pages” artifact). If the repository is not named `note_app`, change `--base=/note_app/` in `package.json` to match your repo name (trailing slash required).

Direct visits to routes like `/note_app/trash` need GitHub to fall back to the SPA: the build copies `index.html` to `404.html` for that purpose.

The hosted app still needs a reachable API: set `VITE_API_URL` when building (e.g. your public backend `/api` URL), since GitHub Pages cannot proxy to `localhost:5207`.

## Notes for reviewers

- The app uses soft-delete (`IsDeleted`) for trash behavior.
- Some older markdown files were kept only as archived pointers; use this README + QUICKSTART as source of truth.
