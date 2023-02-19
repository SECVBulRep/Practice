FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BookShop.WebService/BookShop.WebService.csproj", "BookShop.WebService/"]
RUN dotnet restore "BookShop.WebService/BookShop.WebService.csproj"
COPY . .
WORKDIR "/src/BookShop.WebService"
RUN dotnet build "BookShop.WebService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookShop.WebService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookShop.WebService.dll"]
