FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/ ./src/
COPY *.sln ./
RUN dotnet restore

COPY . ./
RUN dotnet restore src/OurTube.Api/OurTube.Api.csproj
RUN dotnet publish src/OurTube.Api/OurTube.Api.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update && \
    apt-get install -y ffmpeg && \
    apt-get clean && rm -rf /var/lib/apt/lists/*


COPY --from=build /app/out ./

EXPOSE 80

ENTRYPOINT ["dotnet", "OurTube.Api.dll"]

