FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TodoItems/TodoItems.csproj", "TodoItems/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["Kafka.Abstractions/Kafka.Abstractions.csproj", "Kafka.Abstractions/"]
COPY ["Kafka/Kafka.csproj", "Kafka/"]
RUN dotnet restore "TodoItems/TodoItems.csproj"
COPY . .
WORKDIR "/src/TodoItems"
RUN dotnet build "./TodoItems.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TodoItems.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoItems.dll"]
