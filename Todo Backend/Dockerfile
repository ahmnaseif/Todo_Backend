#.NET 8 SDK Linux image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Project file and restore dependencies
COPY Todo_Backend.csproj ./
RUN dotnet restore

# Entire project and build it
COPY . ./
RUN dotnet publish -c Release -o out

# .NET 8 runtime Linux image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Published output from the build stage
COPY --from=build /app/out .

# Expose port 5000 for the API
EXPOSE 5000

# Run the application
ENTRYPOINT ["dotnet", "Todo_Backend.dll"]
