FROM mcr.microsoft.com/dotnet/aspnet:5.0.2-alpine3.12-arm64v8 AS base
EXPOSE 80
EXPOSE 443

RUN ls

COPY /PewPew.WebApp.Server/bin/Release/net5.0/publish/ /code/

RUN ls ./code

WORKDIR /code
ENTRYPOINT ["dotnet", "PewPew.WebApp.Server.dll"]
