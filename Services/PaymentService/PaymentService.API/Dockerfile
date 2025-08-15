# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src


# Copy CSPROJ for restoring
COPY Contracts/ECommerce.Contracts/ECommerce.Contracts.csproj Contracts/ECommerce.Contracts/

COPY Services/PaymentService/PaymentService.Domain/PaymentService.Domain.csproj Services/PaymentService/PaymentService.Domain/
COPY Services/PaymentService/PaymentService.Application/PaymentService.Application.csproj Services/PaymentService/PaymentService.Application/
COPY Services/PaymentService/PaymentService.Infrastructure/PaymentService.Infrastructure.csproj Services/PaymentService/PaymentService.Infrastructure/
COPY Services/PaymentService/PaymentService.API/PaymentService.API.csproj Services/PaymentService/PaymentService.API/
# COPY ["Contracts/ECommerce.Contracts/ECommerce.Contracts.csproj", "Contracts/ECommerce.Contracts/"]

# COPY ["PaymentService.API/PaymentService.API.csproj", "PaymentService.API/"]
# COPY ["PaymentService.Application/PaymentService.Application.csproj", "PaymentService.Application/"]
# COPY ["PaymentService.Infrastructure/PaymentService.Infrastructure.csproj", "PaymentService.Infrastructure/"]
# COPY ["PaymentService.Domain/PaymentService.Domain.csproj", "PaymentService.Domain/"]

# Restore
# RUN dotnet restore "PaymentService.API/PaymentService.API.csproj"
RUN dotnet restore Services/PaymentService/PaymentService.API/PaymentService.API.csproj

# Copy all source code
# COPY . .
COPY Contracts/ECommerce.Contracts/ Contracts/ECommerce.Contracts/
COPY Services/PaymentService/ Services/PaymentService/

# Build
WORKDIR /src/Services/PaymentService/PaymentService.API
RUN dotnet build "PaymentService.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "PaymentService.API.csproj" -c Release -o /app/publish

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.API.dll"]