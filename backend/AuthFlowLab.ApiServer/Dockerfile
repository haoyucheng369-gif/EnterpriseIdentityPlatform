FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/AuthFlowLab.ApiServer/AuthFlowLab.ApiServer.csproj backend/AuthFlowLab.ApiServer/
RUN dotnet restore backend/AuthFlowLab.ApiServer/AuthFlowLab.ApiServer.csproj

COPY backend/AuthFlowLab.ApiServer/ backend/AuthFlowLab.ApiServer/
RUN dotnet publish backend/AuthFlowLab.ApiServer/AuthFlowLab.ApiServer.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AuthFlowLab.ApiServer.dll"]
