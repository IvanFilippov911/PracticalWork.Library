FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["src/PracticalWork.Reports.Worker/PracticalWork.Reports.Worker.csproj", "src/PracticalWork.Reports.Worker/"]
COPY ["src/PracticalWork.Library.Cache.Redis/PracticalWork.Library.Cache.Redis.csproj", "src/PracticalWork.Library.Cache.Redis/"]
COPY ["src/PracticalWork.Library.Contracts/PracticalWork.Library.Contracts.csproj", "src/PracticalWork.Library.Contracts/"]
COPY ["src/PracticalWork.Library.Data.Minio/PracticalWork.Library.Data.Minio.csproj", "src/PracticalWork.Library.Data.Minio/"]
COPY ["src/PracticalWork.Library.Data.Reports.PostgreSql/PracticalWork.Library.Data.Reports.PostgreSql.csproj", "src/PracticalWork.Library.Data.Reports.PostgreSql/"]
COPY ["src/PracticalWork.Library.MessageBroker/PracticalWork.Library.MessageBroker.csproj", "src/PracticalWork.Library.MessageBroker/"]
RUN dotnet restore "./src/PracticalWork.Reports.Worker/PracticalWork.Reports.Worker.csproj"
COPY . .
WORKDIR "/src/src/PracticalWork.Reports.Worker"
RUN dotnet build "./PracticalWork.Reports.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PracticalWork.Reports.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PracticalWork.Reports.Worker.dll"]
