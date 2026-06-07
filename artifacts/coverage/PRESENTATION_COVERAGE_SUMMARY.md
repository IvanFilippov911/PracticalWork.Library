# Слайд: Покрытие Unit-тестами

## Готовый текст для слайда

### Результаты покрытия

| Область | Line Coverage | Branch Coverage | Комментарий |
| --- | ---: | ---: | --- |
| Application + Domain | `79.9%` | `75.0%` | Основная бизнес-логика покрыта unit-тестами |
| Infrastructure | `1.9%` | `0%` | Инфраструктурные адаптеры почти не покрыты |

### Ключевые результаты

- `PracticalWork.Library` — `77.5%`
- `PracticalWork.Library.Contracts` — `73.0%`
- `PracticalWork.Reports.Web` — `100%`
- `PracticalWork.Reports.Worker` — `100%`

### Полностью покрытые ключевые сервисы

- `ReportService`
- `ReportEventHandler`
- `ReportGenerateService`
- `ReportGenerationService`
- `ReturnReminderProcessingService`
- `WeeklyAdminReportProcessingService`
- `WeeklyAdminReportNotificationService`
- `WeeklyAdminReportStorageService`
- `WeeklyAdminReportPeriodService`
- `WeeklyAdminReportCsvBuilder`

### Основные проблемные зоны

- `Redis` адаптеры — `0%`
- `MinIO` адаптеры — `0%`
- `Reports PostgreSQL repositories` — `0%`
- `RabbitMQ infrastructure` — `4.9%`

## Короткий вывод для озвучивания

Покрытие было разделено на два независимых отчёта. Для бизнес-логики `Application + Domain` итоговое line coverage составило `79.9%`, branch coverage `75.0%`. Это означает, что основные сценарии работы сервисов и доменных моделей покрыты unit-тестами. При этом инфраструктурный слой покрыт слабо: `1.9%`, так как адаптеры `Redis`, `MinIO`, `PostgreSQL` и `MessageBroker` требуют либо отдельных unit-тестов на обёртки, либо integration-тестов с внешними зависимостями.

## Что показать на диаграмме

- Столбчатая диаграмма по 2 значениям:
  - `Application + Domain` — `79.9`
  - `Infrastructure` — `1.9`

- Дополнительная диаграмма по сборкам:
  - `PracticalWork.Library` — `77.5`
  - `PracticalWork.Library.Contracts` — `73.0`
  - `PracticalWork.Reports.Web` — `100`
  - `PracticalWork.Reports.Worker` — `100`

## Артефакты для ссылки на слайде

- Основной HTML отчёт: [artifacts/coverage/app-domain/report/index.html](/Users/tagirahmetsin/3%20курс/ак%20барс/PracticalWork.Library/artifacts/coverage/app-domain/report/index.html)
- Infrastructure HTML отчёт: [artifacts/coverage/infrastructure/report/index.html](/Users/tagirahmetsin/3%20курс/ак%20барс/PracticalWork.Library/artifacts/coverage/infrastructure/report/index.html)
