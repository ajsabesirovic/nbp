# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore (copy csproj files first for better layer caching)
COPY src/FitJourney.API/FitJourney.API.csproj src/FitJourney.API/
COPY src/FitJourney.Application/FitJourney.Application.csproj src/FitJourney.Application/
COPY src/FitJourney.Domain/FitJourney.Domain.csproj src/FitJourney.Domain/
COPY src/FitJourney.Infrastructure/FitJourney.Infrastructure.csproj src/FitJourney.Infrastructure/
RUN dotnet restore src/FitJourney.API/FitJourney.API.csproj

# Copy the rest and publish
COPY . .
RUN dotnet publish src/FitJourney.API/FitJourney.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Render/Railway inject PORT; the app reads it (see Program.cs). 8080 is the local default.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FitJourney.API.dll"]
