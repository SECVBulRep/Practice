FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WM.TheGame.Client/WM.TheGame.Client.csproj", "WM.TheGame.Client/"]
RUN dotnet restore "WM.TheGame.Client/WM.TheGame.Client.csproj"
COPY . .
WORKDIR "/src/WM.TheGame.Client"
RUN dotnet build "WM.TheGame.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WM.TheGame.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WM.TheGame.Client.dll"]
