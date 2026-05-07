# ── Stage 1: Tailwind CSS ────────────────────────────────────────────────────
FROM node:22-alpine AS css-build
WORKDIR /client
COPY EExamSystem.Client/package*.json ./
RUN npm ci
COPY EExamSystem.Client/wwwroot/app.css ./wwwroot/app.css
COPY EExamSystem.Client/Components/            ./Components/
RUN npx @tailwindcss/cli -i ./wwwroot/app.css -o ./wwwroot/app.min.css --minify

# ── Stage 2: Build both projects ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore (cached unless .csproj files change)
COPY EExamSystem.sln                                        ./
COPY EExamSystem.Shared/EExamSystem.Shared.csproj          EExamSystem.Shared/
COPY EExamSystem.Api/EExamSystem.Api.csproj                EExamSystem.Api/
COPY EExamSystem.Client/EExamSystem.Client.csproj          EExamSystem.Client/
RUN dotnet restore

# Copy source
COPY EExamSystem.Shared/   EExamSystem.Shared/
COPY EExamSystem.Api/      EExamSystem.Api/
COPY EExamSystem.Client/   EExamSystem.Client/
# Inject freshly-built CSS
COPY --from=css-build /client/wwwroot/app.min.css EExamSystem.Client/wwwroot/app.min.css

# Publish both
RUN dotnet publish EExamSystem.Api/EExamSystem.Api.csproj \
        -c Release -o /app/api --no-restore
RUN dotnet publish EExamSystem.Client/EExamSystem.Client.csproj \
        -c Release -o /app/client --no-restore

# ── Stage 3: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/api    ./api/
COPY --from=build /app/client ./client/

# Both processes in one container (API on 8080, Client on 8081)
COPY entrypoint.sh ./entrypoint.sh
RUN chmod +x ./entrypoint.sh

EXPOSE 8080 8081

ENV ASPNETCORE_ENVIRONMENT=Production
# Client calls API on the same container
ENV ApiBaseUrl=http://localhost:8080

ENTRYPOINT ["./entrypoint.sh"]
