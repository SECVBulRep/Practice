FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TeastApps.WebApi/TeastApps.WebApi.csproj", "TeastApps.WebApi/"]
RUN dotnet restore "TeastApps.WebApi/TeastApps.WebApi.csproj"
COPY . .
WORKDIR "/src/TeastApps.WebApi"
RUN dotnet build "TeastApps.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeastApps.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeastApps.WebApi.dll"]
