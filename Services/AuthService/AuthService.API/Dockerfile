# --- Stage 1: Base ASP.NET runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# --- Stage 2: Build with SDK ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy projects (mind the directory structure!)
COPY ["AuthService.API/AuthService.API.csproj", "AuthService.API/"]
COPY ["AuthService.Application/AuthService.Application.csproj", "AuthService.Application/"]
COPY ["AuthService.Domain/AuthService.Domain.csproj", "AuthService.Domain/"]
COPY ["AuthService.Infrastructure/AuthService.Infrastructure.csproj", "AuthService.Infrastructure/"]

# Restore NuGet packages
RUN dotnet restore "AuthService.API/AuthService.API.csproj"

# Copy all source
COPY . .

# Build API project
WORKDIR "/src/AuthService.API"
RUN dotnet build "AuthService.API.csproj" -c Release -o /app/build

# --- Stage 3: Publish ---
FROM build AS publish
RUN dotnet publish "AuthService.API.csproj" -c Release -o /app/publish

# --- Stage 4: Final image ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AuthService.API.dll"]
