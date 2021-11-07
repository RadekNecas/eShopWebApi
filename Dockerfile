#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /app
COPY *.sln .
COPY ["src/eShopWebApi/*.csproj", "./src/eShopWebApi/"]
COPY ["src/eShopWebApi.Core/*.csproj", "./src/eShopWebApi.Core/"]
COPY ["src/eShopWebApi.Infrastructure/*.csproj", "./src/eShopWebApi.Infrastructure/"]
COPY ["tests/eShopWebApi.InfrastructureTests/*.csproj", "./tests/eShopWebApi.InfrastructureTests/"]
COPY ["tests/eShopWebApi.SharedTests/*.csproj", "./tests/eShopWebApi.SharedTests/"]
RUN dotnet restore
COPY . .
RUN dotnet build

FROM build AS publish
WORKDIR /app/src/eShopWebApi
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eShopWebApi.dll"]