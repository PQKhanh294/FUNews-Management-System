# 1. Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy toàn bộ mã nguồn vào thư mục /src trong container
COPY . .

# Phục hồi các thư viện dựa trên file Solution tổng
RUN dotnet restore "PhanQuocKhanh_SE18D05_A02_BE.sln"

# Chuyển vào thư mục API và Build
WORKDIR "/src/FUNewsManagementSystem.API"
RUN dotnet build "FUNewsManagementSystem.API.csproj" -c Release -o /app/build

# 3. Publish
FROM build AS publish
RUN dotnet publish "FUNewsManagementSystem.API.csproj" -c Release -o /app/publish

# 4. Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FUNewsManagementSystem.API.dll"]