# Quickstart

Use this file if you just want to run the app quickly.

## Option A: Local run (recommended for development)

### Backend

```bash
cd NotesApplication
dotnet restore
dotnet run
```

### Frontend

```bash
cd NotesApplication.Frontend
npm install
npm run dev
```

Then open the frontend URL shown by Vite (usually `http://localhost:5173`).

## Option B: Docker Compose

```bash
docker-compose up -d
```

If your compose file is configured for this environment, it will start frontend/backend/db together.

## Verify quickly

1. Open frontend
2. Register a user
3. Create a note
4. Edit and delete a note
5. Confirm list/search/sort works

## API docs (development)

Built-in OpenAPI JSON endpoint:

- `/openapi/v1.json`

