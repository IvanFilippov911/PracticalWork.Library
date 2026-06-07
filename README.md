# PracticalWork.Library

## Назначение

Репозиторий содержит два .NET 10 микросервиса для практической работы по рефакторингу и тестированию:

- `PracticalWork.Library.Web` — API библиотеки;
- `PracticalWork.Reports.Web` — API отчетов;
- `PracticalWork.Reports.Worker` — фоновая генерация отчетов;
- `PracticalWork.Library.Data.PostgreSql.Migrator` — консольный мигратор для обеих БД.

## Структура решения

- `src/PracticalWork.Library.Web` — входная точка Library API
- `src/PracticalWork.Library` — application/domain логика библиотеки
- `src/PracticalWork.Library.Controllers` — HTTP-контроллеры и FluentValidation для Library API
- `src/PracticalWork.Library.Contracts` — внешние контракты, DTO, events, shared options
- `src/PracticalWork.Library.Data.PostgreSql` — PostgreSQL для Library
- `src/PracticalWork.Library.Data.Reports.PostgreSql` — PostgreSQL для Reports
- `src/PracticalWork.Library.Cache.Redis` — Redis cache
- `src/PracticalWork.Library.Data.Minio` — MinIO storage
- `src/PracticalWork.Library.MessageBroker` — RabbitMQ integration
- `src/PracticalWork.Reports.Web` — входная точка Reports API
- `src/PracticalWork.Reports.Worker` — worker обработки `ReportCreateEvent`
- `utils/PracticalWork.Library.Data.PostgreSql.Migrator` — запуск EF migrations

## Требования

- `.NET SDK 10.0`
- `Docker Desktop`
- свободные порты:
  - `8081` — Library API
  - `8082` — Reports API
  - `5438` — PostgreSQL library
  - `5439` — PostgreSQL reports
  - `6380` — Redis
  - `9003` — MinIO API
  - `9004` — MinIO Console
  - `15672` — RabbitMQ Management
  - `5005` — smtp4dev UI

## Документация API

### XML comments


XML comments приведены к рабочему уровню для публичного API ключевых проектов:

- `PracticalWork.Library.Contracts` — события, контракты репозиториев, кэш и message broker abstractions
- `PracticalWork.Library` — публичные сервисы библиотеки и доменные исключения
- `PracticalWork.Library.MessageBroker` — DI entry points, producer, consumer, обработчики и RabbitMQ infrastructure
- `PracticalWork.Library.Data.PostgreSql` — `AppDbContext`, регистрация репозиториев и migrations runner
- `PracticalWork.Library.Web`, `PracticalWork.Reports.Web`, `PracticalWork.Reports.Worker` — публичные entry points, контроллеры, middleware, validators, worker services

### Swagger

Подключаемые XML-файлы:

- `Library.Web`:
  - `PracticalWork.Library.Contracts.xml`
  - `PracticalWork.Library.Controllers.xml`
- `Reports.Web`:
  - `PracticalWork.Library.Contracts.xml`
  - `PracticalWork.Reports.Web.xml`

Swagger URL при запуске через `docker compose`:

- Library API: [http://localhost:8081/swagger/index.html](http://localhost:8081/swagger/index.html)
- Reports API: [http://localhost:8082/swagger/index.html](http://localhost:8082/swagger/index.html)

## Инфраструктура из docker-compose

Файл: docker-compose.yaml

Состав стенда:

- `postgres-library` — БД сервиса библиотеки
- `postgres-reports` — БД сервиса отчетов
- `redis` — кэш
- `minio` — файловое хранилище
- `rabbitmq` — брокер сообщений
- `smtp4dev` — локальный SMTP для проверки email
- `migrator` — применение миграций
- `library-web` — Library API
- `reports-web` — Reports API
- `reports-worker` — consumer/report generator

Полезные внешние URL:

- MinIO Console: [http://localhost:9004](http://localhost:9004)
- RabbitMQ Management: [http://localhost:15672](http://localhost:15672)
- smtp4dev: [http://localhost:5005](http://localhost:5005)

## Порядок запуска

### Вариант 1. Полный запуск через Docker Compose

Сборка и старт всего стенда:

```bash
docker compose up --build
```

Фоновый запуск:

```bash
docker compose up -d --build
```

Порядок внутри compose такой:

1. Поднимаются `postgres-library` и `postgres-reports`
2. После healthcheck запускается `migrator`
3. Затем стартуют `library-web`, `reports-web`, `reports-worker`
4. Параллельно доступны `redis`, `minio`, `rabbitmq`, `smtp4dev`

Проверка после старта:

```bash
docker compose ps
```

### Вариант 2. Локальный запуск приложений с инфраструктурой в Docker

1. Поднять только внешние зависимости:

```bash
docker compose up -d postgres-library postgres-reports redis minio rabbitmq smtp4dev
```

2. Применить миграции:

```bash
dotnet run --project utils/PracticalWork.Library.Data.PostgreSql.Migrator
```

3. Запустить worker отчетов:

```bash
dotnet run --project src/PracticalWork.Reports.Worker
```

4. Запустить Reports API:

```bash
ASPNETCORE_URLS=http://localhost:8082 ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/PracticalWork.Reports.Web
```

5. Запустить Library API:

```bash
ASPNETCORE_URLS=http://localhost:8081 ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/PracticalWork.Library.Web
```

## Мигратор

Проект мигратора:

- utils/PracticalWork.Library.Data.PostgreSql.Migrator/Program.cs
Что делает:

- применяет миграции `AppDbContext`
- применяет миграции `ReportsDbContext`

Конфигурация мигратора:

- локально: utils/PracticalWork.Library.Data.PostgreSql.Migrator/appsettings.json
- в Docker: переменные окружения docker-compose.yaml
Команда локального запуска:

```bash
dotnet run --project utils/PracticalWork.Library.Data.PostgreSql.Migrator
```

## Команды проверки

Сборка решения:

```bash
dotnet build PracticalWork.Library.sln
```

Проверка Swagger после запуска:

```bash
curl http://localhost:8081/swagger/v1/swagger.json
curl http://localhost:8082/swagger/v1/swagger.json
```

## Примечания

- После миграции на `.NET 10` для контейнерной сборки используются Dockerfile’ы на базе `.NET 10`.
- Если сервисы уже были запущены ранее и изменилась схема БД, лучше пересобрать контейнеры:

```bash
docker compose down
docker compose up --build
```
