FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Marketplace.Api/Marketplace.Api.csproj", "Marketplace.Api/"]
RUN dotnet restore "Marketplace.Api/Marketplace.Api.csproj"
COPY . .
WORKDIR "/src/Marketplace.Api"
RUN dotnet build "Marketplace.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Marketplace.Api.csproj" --sc -p:PublishTrimmed=true -c Release -o /app/publish -r linux-musl-x64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD "./Marketplace.Api"
