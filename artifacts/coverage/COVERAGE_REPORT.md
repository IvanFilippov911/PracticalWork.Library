# COVERAGE REPORT

## Подход

Для соответствия формулировке задания отчёт разделён на 2 независимых набора:

1. `Application + Domain`
2. `Infrastructure`

Причина разделения: если считать общий процент по всем сборкам сразу, инфраструктурные адаптеры `Redis`, `MinIO`, `PostgreSQL`, `MessageBroker` сильно занижают итог и искажают оценку бизнес-логики. Поэтому основной процент для защиты берётся из отчёта `Application + Domain`, а инфраструктура показывается отдельно.

## Фильтры

### Application + Domain

- Run settings: [coverage.app-domain.runsettings](/Users/tagirahmetsin/3%20курс/ак%20барс/PracticalWork.Library/coverage.app-domain.runsettings)
- Включены assembly:
  - `PracticalWork.Library`
  - `PracticalWork.Library.Contracts`
  - `PracticalWork.Reports.Web`
  - `PracticalWork.Reports.Worker`
- Исключены файлы, не относящиеся к `Application/Domain`:
  - `Entry.cs`
  - `Program.cs`
  - `Startup.cs`
  - `MvcSetupExtensions.cs`
  - `Controllers/*.cs`
  - `Configuration/*.cs`
  - `Validations/*.cs`
  - `Services/Email/*.cs`
  - `Jobs/**/*.cs`
  - `ReportConsumerWorker.cs`
  - `Options/*.cs`

### Infrastructure

- Run settings: [coverage.infrastructure.runsettings](/Users/tagirahmetsin/3%20курс/ак%20барс/PracticalWork.Library/coverage.infrastructure.runsettings)
- Включены assembly:
  - `PracticalWork.Library.Cache.Redis`
  - `PracticalWork.Library.Data.Minio`
  - `PracticalWork.Library.Data.PostgreSql`
  - `PracticalWork.Library.Data.Reports.PostgreSql`
  - `PracticalWork.Library.MessageBroker`

## Артефакты

### Application + Domain

- HTML: [artifacts/coverage/app-domain/report/index.html](/Users/tagirahmetsin/3%20курс/ак%20барс/PracticalWork.Library/artifacts/coverage/app-domain/report/index.html)
- XML Cobertura: [artifacts/coverage/app-domain/report/Cobertura.xml](/Users/tagirahmetsin/3%20курс/ак%20барс/PracticalWork.Library/artifacts/coverage/app-domain/report/Cobertura.xml)
- XML Summary: [artifacts/coverage/app-domain/report/Summary.xml](/Users/tagirahmetsin/3%20курс/ак%20барс/PracticalWork.Library/artifacts/coverage/app-domain/report/Summary.xml)
- Markdown Summary: [artifacts/coverage/app-domain/report/SummaryGithub.md](/Users/tagirahmetsin/3%20курс/ак%20барс/PracticalWork.Library/artifacts/coverage/app-domain/report/SummaryGithub.md)

### Infrastructure

- HTML: [artifacts/coverage/infrastructure/report/index.html](/Users/tagirahmetsin/3%20курс/ак%20барс/PracticalWork.Library/artifacts/coverage/infrastructure/report/index.html)
- XML Cobertura: [artifacts/coverage/infrastructure/report/Cobertura.xml](/Users/tagirahmetsin/3%20курс/ак%20барс/PracticalWork.Library/artifacts/coverage/infrastructure/report/Cobertura.xml)
- XML Summary: [artifacts/coverage/infrastructure/report/Summary.xml](/Users/tagirahmetsin/3%20курс/ак%20барс/PracticalWork.Library/artifacts/coverage/infrastructure/report/Summary.xml)
- Markdown Summary: [artifacts/coverage/infrastructure/report/SummaryGithub.md](/Users/tagirahmetsin/3%20курс/ак%20барс/PracticalWork.Library/artifacts/coverage/infrastructure/report/SummaryGithub.md)

## Результаты

### Общий результат по Application + Domain

- Line coverage: `79.9%` (`874 / 1093`)
- Branch coverage: `75.0%` (`117 / 156`)
- Assemblies: `4`

### Общий результат по Infrastructure

- Line coverage: `1.9%` (`12 / 612`)
- Branch coverage: `0%` (`0 / 52`)
- Assemblies: `4`

## Покрытие Domain слоёв

Важно: в текущем репозитории `Domain` не вынесен в отдельную сборку полностью, поэтому оценка строится по domain-моделям и contracts-модулю.

### Domain model coverage

- `PracticalWork.Library.Models.BookModels.Book` — `57.8%`
- `PracticalWork.Library.Models.BookModels.BookBorrow` — `100%`
- `PracticalWork.Library.Models.BookModels.BookCoverUpload` — `75%`
- `PracticalWork.Library.Models.Common.PageRequest` — `100%`
- `PracticalWork.Library.Models.Common.PagedResult<T>` — `0%`
- `PracticalWork.Library.Contracts.Models.ReportModels.Report` — `100%`
- `PracticalWork.Library.Contracts.Models.ReportModels.ActivityLog` — `100%`

### Shared domain/contracts

- `PracticalWork.Library.Contracts` — `73.0%`
- Полностью покрыты:
  - `ActivityLog`
  - `Report`
  - `BookCreatedEvent`
  - `BookArchivedEvent`
  - `BookBorrowedEvent`
  - `BookReturnedEvent`
  - `ReaderCreatedEvent`
  - `ReaderClosedEvent`
  - `ReportCreateEvent`
- Частично покрыты:
  - `CacheManager` — `92.4%`
  - `BaseLibraryEvent` — `25%`

## Покрытие Application слоёв

### Library

- `PracticalWork.Library` — `77.5%`

Ключевые application-сервисы:

- `BookArchiveProcessingService` — `89.8%`
- `BookService` — `65.1%`
- `LibraryService` — `52.7%`
- `ReaderService` — `79.1%`
- `ReturnReminderProcessingService` — `100%`
- `WeeklyAdminReportCsvBuilder` — `100%`
- `WeeklyAdminReportNotificationService` — `100%`
- `WeeklyAdminReportPeriodService` — `100%`
- `WeeklyAdminReportProcessingService` — `100%`
- `WeeklyAdminReportStorageService` — `100%`

### Reports

- `PracticalWork.Reports.Web` — `100%`
- `PracticalWork.Reports.Worker` — `100%`

Ключевые application-компоненты:

- `ReportService` — `100%`
- `ReportEventHandler` — `100%`
- `ReportGenerateService` — `100%`
- `ReportGenerationService` — `100%`

## Покрытие критической логики Infrastructure

### Dedicated infrastructure assemblies

- `PracticalWork.Library.Cache.Redis` — `0%`
- `PracticalWork.Library.Data.Minio` — `0%`
- `PracticalWork.Library.Data.Reports.PostgreSql` — `0%`
- `PracticalWork.Library.MessageBroker` — `4.9%`

### Что это означает

- `Redis` и `MinIO` адаптеры unit-тестами не покрыты.
- Репозитории `Reports.PostgreSql` unit-тестами не покрыты.
- `MessageBroker` покрыт только частично за счёт `LibraryEventHandler`.
- `PracticalWork.Library.Data.PostgreSql` не попал в итоговый infrastructure report как исполненная assembly, то есть тесты её не загружают и не исполняют.

## Непокрытые классы и методы

### Непокрытые классы Application/Domain

- `PracticalWork.Library.Models.Common.PagedResult<T>`
- `PracticalWork.Library.Exceptions.AuthenticationException`
- `PracticalWork.Library.Exceptions.EntityNotFoundException<T>`
- `PracticalWork.Library.Exceptions.NotFoundException`
- `PracticalWork.Library.Contracts.Exceptions.AppException`
- `PracticalWork.Library.Contracts.Exceptions.EntityNotFoundException<T>`
- `PracticalWork.Library.Contracts.Exceptions.NotFoundException`
- `PracticalWork.Library.Contracts.v1.Books.Request.CreateBookRequest`
- `PracticalWork.Library.Contracts.v1.Books.Request.UpdateBookRequest`
- `PracticalWork.Library.Contracts.v1.Books.Response.BookDetailsResponse`
- `PracticalWork.Library.Contracts.v1.Books.Response.BookResponse`
- `PracticalWork.Library.Contracts.v1.Books.Response.BookWithIssuanceRecordsResponse`
- `PracticalWork.Library.Contracts.v1.Reader.Request.CreateReaderRequest`

### Частично покрытые методы и сценарии business-логики

- `BookService`
  - не все ветки обновления и чтения покрыты;
  - остаются непокрытые ветки по валидации и работе с кэшем.
- `LibraryService`
  - неполное покрытие сценариев чтения деталей книги и пагинации.
- `ReaderService`
  - не все негативные и read-only сценарии покрыты.
- `Book`
  - не все доменные ветки переходов состояния и guard-условий покрыты.

### Полностью непокрытые классы Infrastructure

- `PracticalWork.Library.Cache.Redis.Entry`
- `PracticalWork.Library.Cache.Redis.Services.CacheService`
- `PracticalWork.Library.Cache.Redis.Services.CacheVersionService`
- `PracticalWork.Library.Data.Minio.Entry`
- `PracticalWork.Library.Data.Minio.MinioService`
- `PracticalWork.Library.Data.Reports.PostgreSql.Repositories.ActivityLogRepository`
- `PracticalWork.Library.Data.Reports.PostgreSql.Repositories.ReportRepository`
- `PracticalWork.Library.Data.Reports.PostgreSql.ReportsDbContext`
- `PracticalWork.Library.Data.Reports.PostgreSql.ReportsDbContextFactory`
- `PracticalWork.Library.MessageBroker.Entry`
- `PracticalWork.Library.MessageBroker.Producer`
- `PracticalWork.Library.MessageBroker.Utils.RabbitMqInfrastructureInitializer`
- `PracticalWork.Library.MessageBroker.Workers.ConsumerWorker`
- `PracticalWork.Library.MessageBroker.Workers.ProducerWorker`

## Вывод

- Требование по отдельному анализу `Application/Domain` и `Infrastructure` выполнено.
- Для презентации и защиты корректно использовать основной процент `79.9%` как результат покрытия бизнес-логики.
- Инфраструктура покрыта слабо: `1.9%`, что указывает на необходимость adapter-level unit tests или integration tests.

## Следующий шаг

Если нужно поднимать coverage дальше, максимальный практический эффект дадут:

1. unit-тесты для `CacheService`, `CacheVersionService`, `MinioService`;
2. unit/integration-тесты для `ActivityLogRepository` и `ReportRepository`;
3. тесты на `ReportConsumerWorker` и RabbitMQ orchestration.
