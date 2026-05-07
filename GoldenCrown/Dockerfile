FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY "GoldenCrown.csproj" .
# Restore as distinct layers
RUN dotnet restore "GoldenCrown.csproj"
COPY . .
# Build and publish a release
RUN dotnet build "GoldenCrown.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GoldenCrown.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoldenCrown.dll"]