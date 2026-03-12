FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "HousingAPI2/HousingAPI/HousingAPI.csproj"
WORKDIR "/src/HousingAPI2/HousingAPI"
RUN dotnet build "HousingAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HousingAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "HousingAPI.dll"]
