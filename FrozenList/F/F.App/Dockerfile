FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["F.App/F.App.csproj", "F.App/"]
RUN dotnet restore "F.App/F.App.csproj"
COPY . .
WORKDIR "/src/F.App"
RUN dotnet build "F.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "F.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "F.App.dll"]
