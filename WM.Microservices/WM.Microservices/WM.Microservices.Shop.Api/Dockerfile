FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WM.Microservices.Shop.Api/WM.Microservices.Shop.Api.csproj", "WM.Microservices.Shop.Api/"]
RUN dotnet restore "WM.Microservices.Shop.Api/WM.Microservices.Shop.Api.csproj"
COPY . .
WORKDIR "/src/WM.Microservices.Shop.Api"
RUN dotnet build "WM.Microservices.Shop.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WM.Microservices.Shop.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WM.Microservices.Shop.Api.dll"]
