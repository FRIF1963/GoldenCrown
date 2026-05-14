FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["GoldenCrown.Api/*.csproj", "./GoldenCrown.Api/"]
COPY ["GoldenCrown.Application/*.csproj", "./GoldenCrown.Application/"]
COPY ["GoldenCrown.Infrastructure/*.csproj", "./GoldenCrown.Infrastructure/"]
COPY ["GoldenCrown.Domain/*.csproj", "./GoldenCrown.Domain/"]
# Restore as distinct layers
RUN dotnet restore "./GoldenCrown.Api/GoldenCrown.Api.csproj"
COPY . .
# Build and publish a release
RUN dotnet build "./GoldenCrown.Api/GoldenCrown.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./GoldenCrown.Api/GoldenCrown.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoldenCrown.Api.dll"]