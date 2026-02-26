# Етап збірки (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PropertieForLanguages.csproj", "./"]
RUN dotnet restore "PropertieForLanguages.csproj"
COPY . .
RUN dotnet publish "PropertieForLanguages.csproj" -c Release -o /app/publish

# Етап запуску (Run)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
# Копіюємо базу та JSON, щоб вони були доступні на хості
COPY ["words.json", "./"]
# Відкриваємо порт
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "PropertieForLanguages.dll"]