FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HideMethods/HideMethods.csproj", "HideMethods/"]
RUN dotnet restore "HideMethods/HideMethods.csproj"
COPY . .
WORKDIR "/src/HideMethods"
RUN dotnet build "HideMethods.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HideMethods.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HideMethods.dll"]
