# Frontend Build Stage
FROM node:20-alpine as frontend-build
WORKDIR /app

# Copy package files
COPY NotesApplication.Frontend/package*.json ./
RUN npm ci

# Copy source code
COPY NotesApplication.Frontend/ .

# Build
RUN npm run build

# Production Stage - Nginx
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Copy built files
COPY --from=frontend-build /app/dist .

# Copy nginx configuration
COPY NotesApplication.Frontend/nginx.conf /etc/nginx/conf.d/default.conf

# Create non-root user
RUN addgroup -g 101 -S nginx 2>/dev/null || true

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --quiet --tries=1 --spider http://localhost/ || exit 1

CMD ["nginx", "-g", "daemon off;"]
