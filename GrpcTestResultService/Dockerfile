
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081



FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GrpcTestResultService/GrpcTestResultService.csproj", "GrpcTestResultService/"]
COPY ["UserMqDto/UserMqDto.csproj", "UserMqDto/"]
RUN dotnet restore "./GrpcTestResultService/GrpcTestResultService.csproj"
COPY . .
WORKDIR "/src/GrpcTestResultService"
RUN dotnet build "./GrpcTestResultService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GrpcTestResultService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GrpcTestResultService.dll"]