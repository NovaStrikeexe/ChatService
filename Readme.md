# ChatService

## Описание проекта

Проект **ChatService** представляет собой сервис для обмена сообщениями, включающий API и клиентское приложение. Сервис поддерживает отправку и получение сообщений в реальном времени с использованием SignalR и хранение истории сообщений в базе данных PostgreSQL.

## Структура проекта

Проект состоит из следующих основных частей:

- **ChatService**: Основной проект сервера, включающий контроллеры, службы и настройки конфигурации.
  - **ChatService/Configuration**: Содержит классы конфигурации, такие как `DbSettings`, `SwaggerSettings`, и `CorsSettings`.
  - **ChatService/Data**: Содержит реализацию репозитория и модели данных.
  - **ChatService/Http**: Включает HTTP-контракты и DTO для взаимодействия с API.
  - **ChatService/Services**: Реализация бизнес-логики и интерфейсов для работы с сообщениями.
  - **ChatService/SignalR**: Реализует интеграцию с SignalR для обмена сообщениями в реальном времени.

- **ChatService.MessageSenderClient**: Клиентское приложение на Blazor для отправки сообщений в сервис.

- **ChatService.Tests**: Юнит-тесты для проверки функциональности сервиса. Все зависимости замокированы для изоляции тестируемого кода.

- **ChatService.Utils**: Утилиты для управления проектом, включая конфигурационные файлы Docker Compose и скрипты для инициализации базы данных.

## Запуск сервиса

Для запуска сервиса вы можете использовать Docker Compose. Убедитесь, что у вас установлены Docker и Docker Compose. 

1. Настройте конфигурацию в `.env` файле. Пример `.env` файла:

    ```plaintext
    # Порты для сервисов
    CHAT_SERVICE_PORT=5150
    CLIENT_PORT=5068
    DB_PORT=5432

    # Параметры подключения к базе данных
    POSTGRES_DB=chatservice
    POSTGRES_USER=user
    POSTGRES_PASSWORD=password
    ```

2. Запустите Docker Compose из директории `ChatService.Utils`:

    ```bash
    docker-compose up --build
    ```

Это поднимет все необходимые сервисы, включая сервер API, базу данных и клиентское приложение.

## Конфигурация

Конфигурация сервиса ChatService хранится в файле `ChatService/appsettings.json`. Этот файл содержит настройки для логирования, подключения к базе данных, CORS, и Swagger. Пример содержимого `appsettings.json`:

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

Этот файл управляет основными параметрами работы сервиса, такими как подключения к базе данных и настройки API документации.

## Запуск с использованием .NET 8

Альтернативно, вы можете запустить сервис с использованием .NET 8. Для этого необходимо установить .NET 8 SDK и выполнить команду из корневой директории `ChatService`:

```bash
dotnet run --project ChatService
```

Это запустит сервер на порту, указанном в конфигурации или по умолчанию на `http://localhost:5150`.

---

Эта документация должна помочь вам настроить и запустить сервис ChatService для обмена сообщениями.
```