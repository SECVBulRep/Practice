FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Bogus.App/Bogus.App.csproj", "Bogus.App/"]
RUN dotnet restore "Bogus.App/Bogus.App.csproj"
COPY . .
WORKDIR "/src/Bogus.App"
RUN dotnet build "Bogus.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bogus.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bogus.App.dll"]
