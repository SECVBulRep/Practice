FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WM.TheGame.SiloFirst/WM.TheGame.SiloFirst.csproj", "WM.TheGame.SiloFirst/"]
RUN dotnet restore "WM.TheGame.SiloFirst/WM.TheGame.SiloFirst.csproj"
COPY . .
WORKDIR "/src/WM.TheGame.SiloFirst"
RUN dotnet build "WM.TheGame.SiloFirst.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WM.TheGame.SiloFirst.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WM.TheGame.SiloFirst.dll"]
