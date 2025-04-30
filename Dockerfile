# Stage 1: Build .NET backend
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy solution and project files first
COPY *.sln .
COPY Apollo.Api/*.csproj Apollo.Api/
COPY Apollo.Agents/*.csproj Apollo.Agents/
COPY Apollo.Config/*.csproj Apollo.Config/
COPY Apollo.Data/*.csproj Apollo.Data/
COPY Apollo.Search/*.csproj Apollo.Search/
COPY Apollo.Crawler/*.csproj Apollo.Crawler/

# Restore .NET dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Publish API
WORKDIR /source/Apollo.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Configure environment variables for production
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80

ENTRYPOINT ["dotnet", "Apollo.Api.dll"]