FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SentryVision.HubApi/SentryVision.HubApi.csproj", "SentryVision.HubApi/"]
RUN dotnet restore "SentryVision.HubApi/SentryVision.HubApi.csproj"
COPY . .
WORKDIR "/src/SentryVision.HubApi"
RUN dotnet build "SentryVision.HubApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SentryVision.HubApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SentryVision.HubApi.dll"]
