# Stage 1: Build the .NET application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy solution and project files first for layer caching
# Copy .sln file (assuming it's named Apollo.sln, adjust if different)
COPY *.sln .
# Copy project files from their respective directories
COPY Apollo.Api/*.csproj ./Apollo.Api/
COPY Apollo.Agents/*.csproj ./Apollo.Agents/
COPY Apollo.Config/*.csproj ./Apollo.Config/
COPY Apollo.Data/*.csproj ./Apollo.Data/
COPY Apollo.Search/*.csproj ./Apollo.Search/
COPY Apollo.Notifications/*.csproj ./Apollo.Notifications/

# Restore dependencies for the entire solution
RUN dotnet restore "./Apollo.sln"

# Copy the rest of the source code
COPY . .

# Publish the API project in Release configuration
WORKDIR /source/Apollo.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Set environment variables for Render compatibility
# ASP.NET Core automatically listens on the PORT environment variable provided by Render.
# Setting ASPNETCORE_URLS ensures it listens on all interfaces within the container.
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port 80 (Render will map its internal port to this)
EXPOSE 80

# Define the entry point for the application
ENTRYPOINT ["dotnet", "Apollo.Api.dll"]