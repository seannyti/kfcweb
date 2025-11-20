FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MySettings.Api.csproj", "./"]
RUN dotnet restore "MySettings.Api.csproj"
COPY . .
RUN dotnet build "MySettings.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MySettings.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN useradd -m -u 1000 appuser && chown -R appuser:appuser /app
USER appuser
ENTRYPOINT ["dotnet", "MySettings.Api.dll"]
