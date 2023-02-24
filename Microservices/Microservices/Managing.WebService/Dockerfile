FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /App
EXPOSE 80
EXPOSE 443

COPY ["Managing.WebService/.", "Managing.WebService/"]
COPY ["Microservices.SeedWork/.", "Microservices.SeedWork/"]

WORKDIR Managing.WebService
RUN dotnet restore "Managing.WebService.csproj"
RUN dotnet publish -c Release -o /App/build

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /App/build
COPY --from=build /App/build .
ENTRYPOINT ["dotnet", "Managing.WebService.dll"]