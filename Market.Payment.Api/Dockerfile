FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Market.Payment.Api/Market.Payment.Api.csproj", "Market.Payment.Api/"]
RUN dotnet restore "Market.Payment.Api/Market.Payment.Api.csproj"
COPY . .
WORKDIR "/src/Market.Payment.Api"
RUN dotnet build "Market.Payment.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Market.Payment.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Market.Payment.Api.dll"]
