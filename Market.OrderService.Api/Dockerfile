FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Market.OrderService.Api/Market.OrderService.Api.csproj", "Market.OrderService.Api/"]
RUN dotnet restore "Market.OrderService.Api/Market.OrderService.Api.csproj"
COPY . .
WORKDIR "/src/Market.OrderService.Api"
RUN dotnet build "Market.OrderService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Market.OrderService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Market.OrderService.Api.dll"]
