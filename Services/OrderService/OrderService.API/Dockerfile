# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# COPY ["OrderService.API/OrderService.API.csproj", "OrderService.API/"]
# COPY ["OrderService.Application/OrderService.Application.csproj", "OrderService.Application/"]
# COPY ["OrderService.Infrastructure/OrderService.Infrastructure.csproj", "OrderService.Infrastructure/"]
# COPY ["OrderService.Domain/OrderService.Domain.csproj", "OrderService.Domain/"]

# ---- Copy CSPROJ for restoring ----
COPY Contracts/ECommerce.Contracts/ECommerce.Contracts.csproj Contracts/ECommerce.Contracts/

COPY Services/OrderService/OrderService.Domain/OrderService.Domain.csproj Services/OrderService/OrderService.Domain/
COPY Services/OrderService/OrderService.Application/OrderService.Application.csproj Services/OrderService/OrderService.Application/
COPY Services/OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj Services/OrderService/OrderService.Infrastructure/
COPY Services/OrderService/OrderService.API/OrderService.API.csproj Services/OrderService/OrderService.API/

# Restore
# RUN dotnet restore "OrderService.API/OrderService.API.csproj"
RUN dotnet restore Services/OrderService/OrderService.API/OrderService.API.csproj

# Copy all source code
# COPY . .
COPY Contracts/ECommerce.Contracts/ Contracts/ECommerce.Contracts/
COPY Services/OrderService/ Services/OrderService/

# Build
# WORKDIR "/src/OrderService.API"
WORKDIR /src/Services/OrderService/OrderService.API
RUN dotnet build "OrderService.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
# RUN dotnet publish "OrderService.API.csproj" -c Release -o /app/publish
RUN dotnet publish OrderService.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.API.dll"]
