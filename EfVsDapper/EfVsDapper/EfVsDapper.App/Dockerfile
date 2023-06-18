FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["EfVsDapper.App/EfVsDapper.App.csproj", "EfVsDapper.App/"]
RUN dotnet restore "EfVsDapper.App/EfVsDapper.App.csproj"
COPY . .
WORKDIR "/src/EfVsDapper.App"
RUN dotnet build "EfVsDapper.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EfVsDapper.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EfVsDapper.App.dll"]
