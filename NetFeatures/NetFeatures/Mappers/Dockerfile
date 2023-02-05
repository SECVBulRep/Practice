FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Mappers/Mappers.csproj", "Mappers/"]
RUN dotnet restore "Mappers/Mappers.csproj"
COPY . .
WORKDIR "/src/Mappers"
RUN dotnet build "Mappers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mappers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mappers.dll"]
