FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ELK.App/ELK.App.csproj", "ELK.App/"]
RUN dotnet restore "ELK.App/ELK.App.csproj"
COPY . .
WORKDIR "/src/ELK.App"
RUN dotnet build "ELK.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ELK.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ELK.App.dll"]
