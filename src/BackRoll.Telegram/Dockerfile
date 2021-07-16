#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ./BackRoll.sln ./
COPY ./docker-compose.dcproj ./
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done
COPY tests/*/*.csproj ./  
RUN for file in $(ls *.csproj); do mkdir -p tests/${file%.*}/ && mv $file tests/${file%.*}/; done
RUN dotnet restore BackRoll.sln
COPY . .
RUN dotnet build BackRoll.sln -c Release -o /app/build

FROM build AS publish
ARG Spotify__ClientId
ENV Spotify__ClientId ${Spotify__ClientId}
ARG Spotify__ClientSecret
ENV Spotify__ClientSecret ${Spotify__ClientSecret}
ARG YandexMusic__UseProxy
ENV YandexMusic__UseProxy ${YandexMusic__UseProxy}
ARG YandexMusic__ProxyHost
ENV YandexMusic__ProxyHost ${YandexMusic__ProxyHost}
ARG YandexMusic__ProxyPort
ENV YandexMusic__ProxyPort ${YandexMusic__ProxyPort}
ARG YandexMusic__ProxyUserName
ENV YandexMusic__ProxyUserName ${YandexMusic__ProxyUserName}
ARG YandexMusic__ProxyPassword
ENV YandexMusic__ProxyPassword ${YandexMusic__ProxyPassword}
RUN dotnet test BackRoll.sln
RUN dotnet publish "./src/BackRoll.Telegram/BackRoll.Telegram.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackRoll.Telegram.dll"]
