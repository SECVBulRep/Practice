FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /App
EXPOSE 80
EXPOSE 443

COPY ["BookShop.WebService/.", "BookShop.WebService/"]
COPY ["Microservices.SeedWork/.", "Microservices.SeedWork/"]

WORKDIR BookShop.WebService
RUN dotnet restore "BookShop.WebService.csproj"
RUN dotnet publish -c Release -o /App/build

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /App/build
COPY --from=build /App/build .
ENTRYPOINT ["dotnet", "BookShop.WebService.dll"]