FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Managing.WebService/Managing.WebService.csproj", "Managing.WebService/"]
RUN dotnet restore "Managing.WebService/Managing.WebService.csproj"
COPY . .
WORKDIR "/src/Managing.WebService"
RUN dotnet build "Managing.WebService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Managing.WebService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Managing.WebService.dll"]
