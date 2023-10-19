FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine as build
ARG APP_CONFIG=Release
ARG BASE_PATH=.

WORKDIR /src
COPY $BASE_PATH/Market.DAL/Market.DAL.csproj Market.DAL/
COPY $BASE_PATH/Market.Mq/Market.Mq.csproj Market.Mq/
COPY $BASE_PATH/Market.OrderService.Api/Market.OrderService.Api.csproj Market.OrderService.Api/
RUN dotnet restore "Market.OrderService.Api/Market.OrderService.Api.csproj" /p:Configuration=$APP_CONFIG

COPY $BASE_PATH/Market.DAL/. Market.DAL/
COPY $BASE_PATH/Market.Mq/. Market.Mq/
COPY $BASE_PATH/Market.OrderService.Api/. Market.OrderService.Api/
RUN dotnet publish "Market.OrderService.Api/Market.OrderService.Api.csproj" -c $APP_CONFIG -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
RUN apk upgrade musl
RUN adduser --disabled-password --home /app --gecos '' appuser && chown -R appuser /app
USER appuser
WORKDIR /app
COPY --from=build /out ./
ENV ASPNETCORE_URLS=http://*:5000
ENTRYPOINT ["dotnet", "Market.OrderService.Api.dll"]