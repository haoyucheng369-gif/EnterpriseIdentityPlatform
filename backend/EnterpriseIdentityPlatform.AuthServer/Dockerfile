FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/EnterpriseIdentityPlatform.AuthServer/EnterpriseIdentityPlatform.AuthServer.csproj backend/EnterpriseIdentityPlatform.AuthServer/
RUN dotnet restore backend/EnterpriseIdentityPlatform.AuthServer/EnterpriseIdentityPlatform.AuthServer.csproj

COPY backend/EnterpriseIdentityPlatform.AuthServer/ backend/EnterpriseIdentityPlatform.AuthServer/
RUN dotnet publish backend/EnterpriseIdentityPlatform.AuthServer/EnterpriseIdentityPlatform.AuthServer.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "EnterpriseIdentityPlatform.AuthServer.dll"]
