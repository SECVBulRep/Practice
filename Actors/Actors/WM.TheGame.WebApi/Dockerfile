FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WM.TheGame.WebApi/WM.TheGame.WebApi.csproj", "WM.TheGame.WebApi/"]
RUN dotnet restore "WM.TheGame.WebApi/WM.TheGame.WebApi.csproj"
COPY . .
WORKDIR "/src/WM.TheGame.WebApi"
RUN dotnet build "WM.TheGame.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WM.TheGame.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WM.TheGame.WebApi.dll"]
