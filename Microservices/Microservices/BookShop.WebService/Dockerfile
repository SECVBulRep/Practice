FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT="development"
EXPOSE 5000


WORKDIR /App
COPY ["BookShop.WebService/.", "BookShop.WebService/"]
COPY ["Microservices.SeedWork/.", "Microservices.SeedWork/"]


WORKDIR BookShop.WebService
RUN dotnet restore "BookShop.WebService.csproj"
RUN dotnet publish -c Release -o /App/build

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /App/build
COPY --from=build /App/build .
ENTRYPOINT ["dotnet", "BookShop.WebService.dll"]