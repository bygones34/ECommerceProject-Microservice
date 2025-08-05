# 1. Build layer: SDK image is used
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# 2. Copying .csproj files and getting the dependencies.
COPY ProductService.API/*.csproj ./ProductService.API/
COPY ProductService.Application/*.csproj ./ProductService.Application/
COPY ProductService.Domain/*.csproj ./ProductService.Domain/
COPY ProductService.Infrastructure/*.csproj ./ProductService.Infrastructure/

# 3. Restore process on ProductService.API Project.
RUN dotnet restore ./ProductService.API/ProductService.API.csproj

# 4. Copying all the source files
COPY ProductService.API/. ./ProductService.API/
COPY ProductService.Application/. ./ProductService.Application/
COPY ProductService.Domain/. ./ProductService.Domain/
COPY ProductService.Infrastructure/. ./ProductService.Infrastructure/

# 5. Publishing process
RUN dotnet publish ./ProductService.API/ProductService.API.csproj -c Release -o out


# 6. Runtime layer: image necessary only to run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "ProductService.API.dll"]
