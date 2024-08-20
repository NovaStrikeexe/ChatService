# Chat Service

## О проекте

Chat Service - это система обмена сообщениями, реализованная с использованием ASP.NET Core и SignalR. Проект включает несколько компонентов:

- **ChatService**: Основной серверный компонент, обеспечивающий обработку сообщений и взаимодействие через SignalR.
- **ChatService.MessageSenderClient**: Клиентское приложение для отправки сообщений.
- **ChatService.Tests**: Модульные тесты для серверной части и SignalR сервиса.
- **ChatService.Utils**: Утилиты для запуска проекта с помощью Docker Compose.

## Структура проекта

### `ChatService`

Основной проект сервиса, включающий:

- **Controllers**: Контроллеры API, например, `MessagesController`.
- **Models**: Модели данных, такие как `Msg` и `MsgDto`.
- **Services**: Реализация сервисов, включая `SignalRService`.
- **Data**: Репозитории и контексты для работы с базой данных.

### `ChatService.MessageSenderClient`

Клиентское приложение для отправки сообщений, использующее SignalR для взаимодействия с сервером.

### `ChatService.Tests`

Содержит модульные тесты для проверки функциональности сервиса и SignalR взаимодействия. Использует Moq для создания заглушек и проверки методов.

### `ChatService.Utils`

Утилиты для запуска и управления проектом, включая `docker-compose.yml` и `.env` файл.

## Запуск сервиса

Для запуска сервиса используйте Docker Compose. Убедитесь, что у вас установлен Docker и Docker Compose. 

1. Откройте терминал и перейдите в директорию `ChatService.Utils`.
2. Запустите команду:
   ```bash
   docker-compose up
   ```

Эта команда поднимет контейнеры для сервиса и базы данных.

## Конфигурация через `.env`

Файл `.env` используется для настройки переменных окружения для различных сервисов и базы данных. Поместите файл `.env` в директорию `ChatService.Utils` с содержимым:

```dotenv
# Порты для сервисов
CHAT_SERVICE_PORT=5019
CLIENT_PORT=5068
DB_PORT=5432

# Параметры подключения к базе данных
POSTGRES_DB=chatservice
POSTGRES_USER=user
POSTGRES_PASSWORD=password
```

- **CHAT_SERVICE_PORT**: Порт, на котором работает сервис ChatService.
- **CLIENT_PORT**: Порт, на котором работает клиентское приложение.
- **DB_PORT**: Порт для подключения к базе данных PostgreSQL.
- **POSTGRES_DB**: Имя базы данных для сервиса.
- **POSTGRES_USER**: Имя пользователя для подключения к базе данных.
- **POSTGRES_PASSWORD**: Пароль для подключения к базе данных.

Эти параметры будут использоваться при запуске и настройке подключения к базе данных.

## Конфигурация сервиса через `appsettings.json`

Файл `appsettings.json` находится в проекте `ChatService` и содержит основные настройки конфигурации:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "DatabaseSettings": {
    "DefaultConnection": "Host=localhost;Port=5432;Username=user;Password=password;Database=chatservice"
  },
  "Swagger": {
    "Endpoint": "/swagger/v1/swagger.json",
    "Title": "Chat Service API",
    "Version": "v1"
  },
  "Cors": {
    "AllowedOrigin": "http://localhost:5068"
  }
}
```

- **Logging**: Конфигурация уровня логирования.
- **DatabaseSettings**: Настройки подключения к базе данных.
- **Swagger**: Конфигурация Swagger UI.
- **Cors**: Настройки CORS.

## Разработка и тестирование

Для разработки и тестирования используйте стандартные инструменты .NET и библиотеки, такие как xUnit для тестирования и Moq для создания заглушек.