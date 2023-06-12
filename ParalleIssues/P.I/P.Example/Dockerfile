FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["P.Example/P.Example.csproj", "P.Example/"]
RUN dotnet restore "P.Example/P.Example.csproj"
COPY . .
WORKDIR "/src/P.Example"
RUN dotnet build "P.Example.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "P.Example.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "P.Example.dll"]
