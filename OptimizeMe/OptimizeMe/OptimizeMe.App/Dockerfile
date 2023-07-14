FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OptimizeMe.App/OptimizeMe.App.csproj", "OptimizeMe.App/"]
RUN dotnet restore "OptimizeMe.App/OptimizeMe.App.csproj"
COPY . .
WORKDIR "/src/OptimizeMe.App"
RUN dotnet build "OptimizeMe.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OptimizeMe.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OptimizeMe.App.dll"]
