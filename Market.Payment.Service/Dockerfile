FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine as build
ARG APP_CONFIG=Release
ARG BASE_PATH=.

WORKDIR /src
COPY $BASE_PATH/Market.Mq/Market.Mq.csproj Market.Mq/
COPY $BASE_PATH/Market.Payment.Service/Market.Payment.Service.csproj Market.Payment.Service/
RUN dotnet restore "Market.Payment.Service/Market.Payment.Service.csproj" /p:Configuration=$APP_CONFIG

COPY $BASE_PATH/Market.Mq/. Market.Mq/
COPY $BASE_PATH/Market.Payment.Service/. Market.Payment.Service/
RUN dotnet publish "Market.Payment.Service/Market.Payment.Service.csproj" -c $APP_CONFIG -o /out --no-restore

FROM mcr.microsoft.com/dotnet/runtime:7.0-alpine
RUN apk upgrade musl
RUN adduser --disabled-password --home /app --gecos '' appuser && chown -R appuser /app
USER appuser
WORKDIR /app
COPY --from=build /out ./

ENTRYPOINT ["dotnet", "Market.Payment.Service.dll"]