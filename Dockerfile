# Етап збірки (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Копіюємо файл проекту з підпапки
# Шлях тепер включає назву папки проєкту
COPY ["PropertieForLanguages/PropertieForLanguages.csproj", "PropertieForLanguages/"]

# 2. Відновлюємо залежності, вказуючи шлях до скопійованого файлу
RUN dotnet restore "PropertieForLanguages/PropertieForLanguages.csproj"

# 3. Копіюємо всі інші файли (включаючи папку PropertieForLanguages та бібліотеки)
COPY . .

# 4. Переходимо в папку з основним проектом для публікації
WORKDIR "/src/PropertieForLanguages"
RUN dotnet publish "PropertieForLanguages.csproj" -c Release -o /app/publish

# Етап запуску (Run)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# 5. Копіюємо words.json. 
# Оскільки на етапі build ми скопіювали все в /src, 
# шлях до файлу буде /src/PropertieForLanguages/words.json
COPY --from=build /src/PropertieForLanguages/words.json .

# Відкриваємо порт
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "PropertieForLanguages.dll"]