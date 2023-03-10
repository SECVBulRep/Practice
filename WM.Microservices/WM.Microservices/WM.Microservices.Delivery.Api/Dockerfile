FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /App
EXPOSE 80
EXPOSE 443

COPY ["WM.Microservices.Delivery.Api/.", "WM.Microservices.Delivery.Api/"]
COPY ["WM.Microservices.Seedwork/.", "WM.Microservices.Seedwork/"]

WORKDIR WM.Microservices.Delivery.Api
RUN dotnet restore "WM.Microservices.Delivery.Api.csproj"
RUN dotnet publish -c Release -o /App/build

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /App/build
COPY --from=build /App/build .
ENTRYPOINT ["dotnet", "WM.Microservices.Delivery.Api.dll"]
