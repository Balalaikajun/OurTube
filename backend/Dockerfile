# Сборка
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.sln ./
COPY src/OurTube.Api/OurTube.Api.csproj ./src/OurTube.Api/
COPY src/OurTube.Infrastructure/OurTube.Infrastructure.csproj ./src/OurTube.Infrastructure/
COPY src/OurTube.Application/OurTube.Application.csproj ./src/OurTube.Application/
COPY src/OurTube.Domain/OurTube.Domain.csproj ./src/OurTube.Domain/

RUN dotnet restore

COPY . .

RUN dotnet publish src/OurTube.Api/OurTube.Api.csproj -c Release -o /app/out

# Рантайм
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN apt-get update && \
    apt-get install -y ffmpeg && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

EXPOSE 8080

COPY --from=build /app/out ./

CMD ["dotnet", "OurTube.Api.dll"]
