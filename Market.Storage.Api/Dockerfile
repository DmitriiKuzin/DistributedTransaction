FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Market.Storage.Api/Market.Storage.Api.csproj", "Market.Storage.Api/"]
RUN dotnet restore "Market.Storage.Api/Market.Storage.Api.csproj"
COPY . .
WORKDIR "/src/Market.Storage.Api"
RUN dotnet build "Market.Storage.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Market.Storage.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Market.Storage.Api.dll"]
