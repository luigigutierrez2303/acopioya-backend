FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY AcopioYA.Api/*.csproj AcopioYA.Api/
RUN dotnet restore AcopioYA.Api/AcopioYA.Api.csproj
COPY AcopioYA.Api/ AcopioYA.Api/
WORKDIR /src/AcopioYA.Api
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "AcopioYA.Api.dll"]
