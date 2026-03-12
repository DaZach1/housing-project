FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN CSPROJ=$(find . -name "HousingAPI.csproj" -type f | head -1) && \
    echo "Found csproj at: $CSPROJ" && \
    dotnet restore "$CSPROJ"
RUN CSPROJ=$(find . -name "HousingAPI.csproj" -type f | head -1) && \
    PROJDIR=$(dirname "$CSPROJ") && \
    cd "$PROJDIR" && \
    dotnet build "HousingAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN CSPROJ=$(find . -name "HousingAPI.csproj" -type f | head -1) && \
    PROJDIR=$(dirname "$CSPROJ") && \
    cd "$PROJDIR" && \
    dotnet publish "HousingAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "HousingAPI.dll"]
