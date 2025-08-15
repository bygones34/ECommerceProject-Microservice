# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy CSPROJ for restoring
# COPY ["InventoryService.API/InventoryService.API.csproj", "InventoryService.API/"]
# COPY ["InventoryService.Application/InventoryService.Application.csproj", "InventoryService.Application/"]
# COPY ["InventoryService.Infrastructure/InventoryService.Infrastructure.csproj", "InventoryService.Infrastructure/"]
# COPY ["InventoryService.Domain/InventoryService.Domain.csproj", "InventoryService.Domain/"]
COPY Contracts/ECommerce.Contracts/ECommerce.Contracts.csproj Contracts/ECommerce.Contracts/

COPY Services/InventoryService/InventoryService.Domain/InventoryService.Domain.csproj Services/InventoryService/InventoryService.Domain/
COPY Services/InventoryService/InventoryService.Application/InventoryService.Application.csproj Services/InventoryService/InventoryService.Application/
COPY Services/InventoryService/InventoryService.Infrastructure/InventoryService.Infrastructure.csproj Services/InventoryService/InventoryService.Infrastructure/
COPY Services/InventoryService/InventoryService.API/InventoryService.API.csproj Services/InventoryService/InventoryService.API/

# Restore
# RUN dotnet restore "InventoryService.API/InventoryService.API.csproj"
RUN dotnet restore Services/InventoryService/InventoryService.API/InventoryService.API.csproj



# Copy all source code
# COPY . .
COPY Contracts/ECommerce.Contracts/ Contracts/ECommerce.Contracts/
COPY Services/InventoryService/ Services/InventoryService/

# Build
# WORKDIR "/src/InventoryService.API"
WORKDIR /src/Services/InventoryService/InventoryService.API
RUN dotnet build "InventoryService.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "InventoryService.API.csproj" -c Release -o /app/publish

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.API.dll"]