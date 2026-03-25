# Backend Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 as backend-build
WORKDIR /src

# Copy project files
COPY ["NotesApplication/NotesApplication.csproj", "NotesApplication/"]
RUN dotnet restore "NotesApplication/NotesApplication.csproj"

# Copy source code
COPY ["NotesApplication/", "NotesApplication/"]
WORKDIR "/src/NotesApplication"

# Build and publish
RUN dotnet build "NotesApplication.csproj" -c Release -o /app/build
RUN dotnet publish "NotesApplication.csproj" -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Install SQL Server tools (optional, for migrations)
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=backend-build /app/publish .

# Create a non-root user for security
RUN useradd -m -u 1001 dotnetuser && chown -R dotnetuser:dotnetuser /app
USER dotnetuser

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

ENTRYPOINT ["dotnet", "NotesApplication.dll"]
