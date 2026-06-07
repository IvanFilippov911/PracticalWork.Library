# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["src/PracticalWork.Library.Web/PracticalWork.Library.Web.csproj", "src/PracticalWork.Library.Web/"]
COPY ["src/PracticalWork.Library.Cache.Redis/PracticalWork.Library.Cache.Redis.csproj", "src/PracticalWork.Library.Cache.Redis/"]
COPY ["src/PracticalWork.Library.Controllers/PracticalWork.Library.Controllers.csproj", "src/PracticalWork.Library.Controllers/"]
COPY ["src/PracticalWork.Library.Contracts/PracticalWork.Library.Contracts.csproj", "src/PracticalWork.Library.Contracts/"]
COPY ["src/PracticalWork.Library.Data.Minio/PracticalWork.Library.Data.Minio.csproj", "src/PracticalWork.Library.Data.Minio/"]
COPY ["src/PracticalWork.Library.Data.PostgreSql/PracticalWork.Library.Data.PostgreSql.csproj", "src/PracticalWork.Library.Data.PostgreSql/"]
COPY ["src/PracticalWork.Library.MessageBroker/PracticalWork.Library.MessageBroker.csproj", "src/PracticalWork.Library.MessageBroker/"]
COPY ["src/PracticalWork.Library/PracticalWork.Library.csproj", "src/PracticalWork.Library/"]
RUN dotnet restore "./src/PracticalWork.Library.Web/PracticalWork.Library.Web.csproj"
COPY . .
WORKDIR "/src/src/PracticalWork.Library.Web"
RUN dotnet build "./PracticalWork.Library.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PracticalWork.Library.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PracticalWork.Library.Web.dll"]
