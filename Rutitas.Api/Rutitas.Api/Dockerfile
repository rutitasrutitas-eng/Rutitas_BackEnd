# Build context: Backend/Rutitas.Api (solution folder)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Rutitas.Api/Rutitas.Api.csproj", "Rutitas.Api/"]
COPY ["Rutitas.Application/Rutitas.Application.csproj", "Rutitas.Application/"]
COPY ["Rutitas.Domain/Rutitas.Domain.csproj", "Rutitas.Domain/"]
COPY ["Rutitas.Infrastructure/Rutitas.Infrastructure.csproj", "Rutitas.Infrastructure/"]
RUN dotnet restore "Rutitas.Api/Rutitas.Api.csproj"
COPY . .
RUN dotnet publish "Rutitas.Api/Rutitas.Api.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Rutitas.Api.dll"]
