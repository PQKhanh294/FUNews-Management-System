# 1. Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8081 
# Dùng port 8081 để không đụng hàng với BE

# 2. Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "PhanQuocKhanh_SE18D05_A02_FE.sln"

WORKDIR "/src/FUNewsManagementSystem.WebClient"
RUN dotnet build "FUNewsManagementSystem.WebClient.csproj" -c Release -o /app/build

# 3. Publish
FROM build AS publish
RUN dotnet publish "FUNewsManagementSystem.WebClient.csproj" -c Release -o /app/publish

# 4. Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FUNewsManagementSystem.WebClient.dll"]