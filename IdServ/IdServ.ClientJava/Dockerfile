FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["IdServ.ClientJava/IdServ.ClientJava.csproj", "IdServ.ClientJava/"]
RUN dotnet restore "IdServ.ClientJava/IdServ.ClientJava.csproj"
COPY . .
WORKDIR "/src/IdServ.ClientJava"
RUN dotnet build "IdServ.ClientJava.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdServ.ClientJava.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdServ.ClientJava.dll"]
