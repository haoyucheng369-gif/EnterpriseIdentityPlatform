FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/EnterpriseIdentityPlatform.Bff/EnterpriseIdentityPlatform.Bff.csproj backend/EnterpriseIdentityPlatform.Bff/
RUN dotnet restore backend/EnterpriseIdentityPlatform.Bff/EnterpriseIdentityPlatform.Bff.csproj

COPY backend/EnterpriseIdentityPlatform.Bff/ backend/EnterpriseIdentityPlatform.Bff/
RUN dotnet publish backend/EnterpriseIdentityPlatform.Bff/EnterpriseIdentityPlatform.Bff.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "EnterpriseIdentityPlatform.Bff.dll"]
