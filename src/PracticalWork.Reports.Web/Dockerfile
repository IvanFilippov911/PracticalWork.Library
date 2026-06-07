FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["src/PracticalWork.Reports.Web/PracticalWork.Reports.Web.csproj", "src/PracticalWork.Reports.Web/"]
COPY ["src/PracticalWork.Library.Cache.Redis/PracticalWork.Library.Cache.Redis.csproj", "src/PracticalWork.Library.Cache.Redis/"]
COPY ["src/PracticalWork.Library.Contracts/PracticalWork.Library.Contracts.csproj", "src/PracticalWork.Library.Contracts/"]
COPY ["src/PracticalWork.Library.Data.Minio/PracticalWork.Library.Data.Minio.csproj", "src/PracticalWork.Library.Data.Minio/"]
COPY ["src/PracticalWork.Library.Data.Reports.PostgreSql/PracticalWork.Library.Data.Reports.PostgreSql.csproj", "src/PracticalWork.Library.Data.Reports.PostgreSql/"]
COPY ["src/PracticalWork.Library.MessageBroker/PracticalWork.Library.MessageBroker.csproj", "src/PracticalWork.Library.MessageBroker/"]
RUN dotnet restore "./src/PracticalWork.Reports.Web/PracticalWork.Reports.Web.csproj"
COPY . .
WORKDIR "/src/src/PracticalWork.Reports.Web"
RUN dotnet build "./PracticalWork.Reports.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PracticalWork.Reports.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PracticalWork.Reports.Web.dll"]
