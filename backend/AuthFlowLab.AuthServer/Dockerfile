FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/AuthFlowLab.AuthServer/AuthFlowLab.AuthServer.csproj backend/AuthFlowLab.AuthServer/
RUN dotnet restore backend/AuthFlowLab.AuthServer/AuthFlowLab.AuthServer.csproj

COPY backend/AuthFlowLab.AuthServer/ backend/AuthFlowLab.AuthServer/
RUN dotnet publish backend/AuthFlowLab.AuthServer/AuthFlowLab.AuthServer.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AuthFlowLab.AuthServer.dll"]
