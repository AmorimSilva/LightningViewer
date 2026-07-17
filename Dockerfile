FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src

COPY ["LightningViewer.Web/LightningViewer.Web.csproj", "LightningViewer.Web/"]
RUN dotnet restore "LightningViewer.Web/LightningViewer.Web.csproj"

COPY ["LightningViewer.Web/", "LightningViewer.Web/"]
WORKDIR /src/LightningViewer.Web
RUN dotnet publish "LightningViewer.Web.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build --chown=app:app /app/publish .

USER app
EXPOSE 8080

ENTRYPOINT ["dotnet", "LightningViewer.Web.dll"]
