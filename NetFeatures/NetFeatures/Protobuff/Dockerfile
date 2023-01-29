FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Protobuff/Protobuff.csproj", "Protobuff/"]
RUN dotnet restore "Protobuff/Protobuff.csproj"
COPY . .
WORKDIR "/src/Protobuff"
RUN dotnet build "Protobuff.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Protobuff.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Protobuff.dll"]
