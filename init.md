# PracticalWork.Library — init

## Назначение
Система управления библиотекой с разделением на микросервисы:
Library отвечает за книги и читателей, Reports за журналы событий и CSV отчеты.

## Быстрый старт (Docker, одна команда)
1. Экспортировать переменные BuildKit (ускоряет сборку):
   ```bash
   export DOCKER_BUILDKIT=1
   export COMPOSE_DOCKER_CLI_BUILD=1
   ```
2. Поднять все сервисы, инфраструктуру и применить миграции:
   ```bash
   docker-compose up -d --build library-stack
   ```
3. Проверить состояние:
   ```bash
   docker-compose ps
   ```

Логи мигратора:
```bash
docker-compose logs --tail=200 migrator
```

Повторно применить миграции:
```bash
docker-compose run --rm migrator
```

## Сервисы приложения
- Library Web API: `src/PracticalWork.Library.Web` (ASP.NET Core 8).
- Reports Web API: `src/PracticalWork.Reports.Web` (ASP.NET Core 8).
- Reports Worker: `src/PracticalWork.Reports.Worker` (обработка событий библиотеки и генерация отчетов).
- Migrator: `utils/PracticalWork.Library.Data.PostgreSql.Migrator` (миграции двух БД).

## Инфраструктура
- PostgreSQL: две БД `library` и `reports`.
- Redis: кэш страниц и деталей.
- MinIO: хранение обложек книг и CSV отчетов.
- RabbitMQ (EasyNetQ): события между сервисами.

## Структура решения
- `src/PracticalWork.Library` — доменная логика, модели, сервисы.
- `src/PracticalWork.Library.Contracts` — контракты API (request/response).
- `src/PracticalWork.Library.Controllers` — контроллеры Library API.
- `src/PracticalWork.Library.Web` — хостинг Library API.
- `src/PracticalWork.Reports.Web` — хостинг Reports API.
- `src/PracticalWork.Library.Data.PostgreSql` — доступ к основной БД.
- `src/PracticalWork.Library.Data.Reports.PostgreSql` — доступ к БД отчетов и логов.
- `src/PracticalWork.Library.Cache.Redis` — кэширование.
- `src/PracticalWork.Library.Data.Minio` — MinIO интеграция.
- `src/PracticalWork.Library.MessageBroker` — RabbitMQ интеграция.
- `src/PracticalWork.Reports.Worker` — worker отчетов.
- `utils/PracticalWork.Library.Data.PostgreSql.Migrator` — мигратор.

## Архитектура и потоки
1. **Доменные события (Library)**
   - Сервисы книг, читателей и выдачи публикуют события.
   - Reports Worker подписывается на события и пишет `ActivityLog` в reports БД.
2. **Генерация отчетов (Reports)**
   - Reports API создает запись отчета и публикует `ReportCreateEvent`.
   - Reports Worker формирует CSV, сохраняет в MinIO, обновляет статус отчета и сбрасывает кэш.

## API маршруты
### Library API (порт 8081)
- `POST /api/v1/books` — создать книгу.
- `PUT /api/v1/books/{id}` — обновить книгу.
- `POST /api/v1/books/{id}/archive` — архивировать.
- `GET /api/v1/books` — список с фильтрами.
- `POST /api/v1/books/{id}/details` — описание и обложка (multipart).
- `POST /api/v1/readers` — создать читателя.
- `POST /{id}/extend` — продлить карточку (абсолютный маршрут).
- `POST /{id}/close` — закрыть карточку (абсолютный маршрут).
- `GET /{id}/books` — книги читателя (абсолютный маршрут).
- `POST /borrow/{bookId}/{readerId}` — выдача (абсолютный маршрут).
- `POST /return/{bookId}/{readerId}` — возврат (абсолютный маршрут).
- `GET /books/{idOrTitle}/details` — детали (абсолютный маршрут).
- `POST /books` — страница неархивных книг (абсолютный маршрут).

### Reports API (порт 8082)
- `POST /api/v1/reports/activity` — страница activity логов.
- `POST /api/v1/reports/generate` — создать CSV отчет.
- `GET /api/v1/reports` — список готовых отчетов.
- `GET /api/v1/reports/{reportName}/download` — presigned URL.

Swagger:
- `http://localhost:8081/swagger`
- `http://localhost:8082/swagger`

## Конфигурация
Общие ключи (полный список для dev см. `docker-compose.yaml`):
- `GlobalPrefix` — префикс пути для всех маршрутов (опционально).
- `App:ConnectionStrings:AppDbContext` — БД библиотеки.
- `App:ConnectionStrings:ReportsDbContext` — БД отчетов.
- `App:Minio:Endpoint` — внутренний endpoint MinIO для сервисов.
- `App:Minio:PublicEndpoint` — endpoint для presigned URL, доступный клиенту.
- `App:Minio:AccessKey`, `App:Minio:SecretKey`.
- `App:Minio:CoversBucketName`, `App:Minio:ReportsBucketName`.
- `App:Minio:ExpInSec` — TTL presigned URL.
- `App:Redis:RedisCacheConnection`, `App:Redis:RedisCachePrefix`.
- `App:RabbitMQ:*` — хост, креды и ключи очередей.

## Docker сервисы и порты
- Library API: `http://localhost:8081`
- Reports API: `http://localhost:8082`
- MinIO API: `http://localhost:9003`
- MinIO Console: `http://localhost:9004`
- PostgreSQL library: `localhost:5438`
- PostgreSQL reports: `localhost:5439`
- Redis: `localhost:6380`
- RabbitMQ: `localhost:5672`
- RabbitMQ UI: `http://localhost:15672`

Credentials (dev):
- PostgreSQL: `postgres / postgres`
- MinIO: `AKIAIOSFODNN7EXAMPLE / wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY`
- RabbitMQ: `guest / guest`

## MinIO и presigned URL
Сервисы используют `App:Minio:Endpoint` для загрузки файлов из контейнера.
`App:Minio:PublicEndpoint` используется для формирования URL, доступного снаружи.
В docker-compose `Endpoint` указывает на `host.docker.internal:9003`, а `PublicEndpoint` на `http://localhost:9003`.

## Миграции
Контейнер `migrator` применяет миграции обеих БД и является обязательной зависимостью для сервисов.
В `docker-compose` это обеспечено через `depends_on` с `service_completed_successfully`.

## Postman
Коллекция: `postman_collection.json`.
Переменные:
- `libraryUrl`: `http://localhost:8081`
- `reportsUrl`: `http://localhost:8082`
- `bookId`, `readerId`, `reportName` — заполняются из ответов.

## Технические особенности
- Версионирование API через URL сегмент `v1`.
- Exceptions: `DomainExceptionFilter` + `AppException`.
- Кэш: `CacheManager` и `CacheVersionService` с инвалидацией по версиям.
- Reports Web регистрирует только `ReportController` (ограничение ApplicationParts).
- В `ReaderController` и `LibraryController` есть абсолютные маршруты, они не содержат `/api/v1`.
