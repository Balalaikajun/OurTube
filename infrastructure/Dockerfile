# ---------- Stage 1: Build Frontend ----------
FROM node:18-alpine AS frontend-build

WORKDIR /app
COPY frontend/package*.json ./
RUN npm ci

COPY frontend/ .
ARG VITE_API_BASE_URL
ARG VITE_MINIO_BASE_URL
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
ENV VITE_MINIO_BASE_URL=$VITE_MINIO_BASE_URL
RUN npm run build        

# ---------- Stage 2: Build & Publish .NET API ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build

WORKDIR /src

# 1) Копируем решение + всё дерево исходников бэкенда
COPY backend/*.sln ./
COPY backend/src/ ./src/

# 2) Восстанавливаем зависимости
RUN dotnet restore

# 3) Копируем собранный фронт в wwwroot
COPY --from=frontend-build /app/dist/ src/OurTube.Api/wwwroot/

# 4) Публикуем в Release
RUN dotnet publish src/OurTube.Api/OurTube.Api.csproj \
    -c Release \
    -o /app/publish

# ---------- Stage 3: Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
RUN apt-get update \
 && apt-get install -y ffmpeg \
 && apt-get clean \
 && rm -rf /var/lib/apt/lists/*

# Копируем результат публикации
COPY --from=dotnet-build /app/publish/ ./

EXPOSE 80
ENTRYPOINT ["dotnet", "OurTube.Api.dll"]
