FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Benchy/Benchy.csproj", "Benchy/"]
RUN dotnet restore "Benchy/Benchy.csproj"
COPY . .
WORKDIR "/src/Benchy"
RUN dotnet build "Benchy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Benchy.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Benchy.dll"]
