FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CircuitBreak/CircuitBreak.csproj", "CircuitBreak/"]
RUN dotnet restore "./CircuitBreak/./CircuitBreak.csproj"
COPY . .
WORKDIR "/src/CircuitBreak"
RUN dotnet build "./CircuitBreak.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CircuitBreak.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CircuitBreak.dll"]