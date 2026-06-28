FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/EnterpriseIdentityPlatform.ApiServer/EnterpriseIdentityPlatform.ApiServer.csproj backend/EnterpriseIdentityPlatform.ApiServer/
RUN dotnet restore backend/EnterpriseIdentityPlatform.ApiServer/EnterpriseIdentityPlatform.ApiServer.csproj

COPY backend/EnterpriseIdentityPlatform.ApiServer/ backend/EnterpriseIdentityPlatform.ApiServer/
RUN dotnet publish backend/EnterpriseIdentityPlatform.ApiServer/EnterpriseIdentityPlatform.ApiServer.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "EnterpriseIdentityPlatform.ApiServer.dll"]
