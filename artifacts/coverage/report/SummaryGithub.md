# Summary
<details open><summary>Summary</summary>

|||
|:---|:---|
| Generated on: | 26.05.2026 - 15:57:41 |
| Coverage date: | 26.05.2026 - 15:57:34 |
| Parser: | MultiReport (2x Cobertura) |
| Assemblies: | 8 |
| Classes: | 98 |
| Files: | 98 |
| **Line coverage:** | 38.3% (930 of 2422) |
| Covered lines: | 930 |
| Uncovered lines: | 1492 |
| Coverable lines: | 2422 |
| Total lines: | 5069 |
| **Branch coverage:** | 43.5% (122 of 280) |
| Covered branches: | 122 |
| Total branches: | 280 |
| **Method coverage:** | [Feature is only available for sponsors](https://reportgenerator.io/pro) |

</details>

## Coverage
<details><summary>PracticalWork.Library - 53.2%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**PracticalWork.Library**|**53.2%**|**53.5%**|
|PracticalWork.Library.Abstractions.Storage.EntityBase|0%||
|PracticalWork.Library.Entry|0%||
|PracticalWork.Library.Exceptions.AppException|66.6%||
|PracticalWork.Library.Exceptions.AuthenticationException|0%||
|PracticalWork.Library.Exceptions.BookServiceException|100%|50%|
|PracticalWork.Library.Exceptions.EntityNotFoundException`1|0%||
|PracticalWork.Library.Exceptions.LibraryServiceException|50%||
|PracticalWork.Library.Exceptions.NotFoundException|0%||
|PracticalWork.Library.Exceptions.ReaderServiceException|50%||
|PracticalWork.Library.Jobs.Archive.ArchiveOldBooksJob|0%|0%|
|PracticalWork.Library.Jobs.Common.CronExpressionValidator|0%|0%|
|PracticalWork.Library.Jobs.Common.HangfireJobManagementService|0%|0%|
|PracticalWork.Library.Jobs.Common.HangfireRecurringJobsHostedService|0%|0%|
|PracticalWork.Library.Jobs.Common.JobExecutionPolicy|0%|0%|
|PracticalWork.Library.Jobs.Common.LibraryJobDescriptions|0%|0%|
|PracticalWork.Library.Jobs.Common.LibraryJobNames|0%||
|PracticalWork.Library.Jobs.Notifications.ReturnReminderJob|0%|0%|
|PracticalWork.Library.Jobs.Reports.WeeklyAdminReportJob|0%|0%|
|PracticalWork.Library.Models.BookModels.Book|57.8%|50%|
|PracticalWork.Library.Models.BookModels.BookBorrow|100%|100%|
|PracticalWork.Library.Models.BookModels.BookCoverUpload|75%||
|PracticalWork.Library.Models.Common.PagedResult`1|0%|0%|
|PracticalWork.Library.Models.Common.PageRequest|100%|50%|
|PracticalWork.Library.Options.JobSettings|0%||
|PracticalWork.Library.Services.BookArchiveProcessingService|89.8%|80%|
|PracticalWork.Library.Services.BookService|65.1%|66.6%|
|PracticalWork.Library.Services.Email.EmailTemplateRenderer|0%|0%|
|PracticalWork.Library.Services.Email.MailKitEmailService|0%|0%|
|PracticalWork.Library.Services.Email.ReturnReminderEmailBuilder|0%||
|PracticalWork.Library.Services.Email.WeeklyAdminReportEmailBuilder|0%||
|PracticalWork.Library.Services.LibraryService|52.7%|60%|
|PracticalWork.Library.Services.ReaderService|79.1%|75%|
|PracticalWork.Library.Services.ReturnReminderProcessingService|100%|100%|
|PracticalWork.Library.Services.WeeklyAdminReportCsvBuilder|100%||
|PracticalWork.Library.Services.WeeklyAdminReportNotificationService|100%|100%|
|PracticalWork.Library.Services.WeeklyAdminReportPeriodService|100%||
|PracticalWork.Library.Services.WeeklyAdminReportProcessingService|100%||
|PracticalWork.Library.Services.WeeklyAdminReportStorageService|100%|100%|

</details>
<details><summary>PracticalWork.Library.Cache.Redis - 0%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**PracticalWork.Library.Cache.Redis**|**0%**|**0%**|
|PracticalWork.Library.Cache.Redis.Entry|0%|0%|
|PracticalWork.Library.Cache.Redis.Services.CacheService|0%|0%|
|PracticalWork.Library.Cache.Redis.Services.CacheVersionService|0%|0%|

</details>
<details><summary>PracticalWork.Library.Contracts - 73%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**PracticalWork.Library.Contracts**|**73%**|**74%**|
|PracticalWork.Library.Contracts.Abstractions.Storage.EntityBase|0%||
|PracticalWork.Library.Contracts.Events.BaseEvent|100%||
|PracticalWork.Library.Contracts.Events.BaseLibraryEvent|25%||
|PracticalWork.Library.Contracts.Events.BookArchivedEvent|100%||
|PracticalWork.Library.Contracts.Events.BookBorrowedEvent|100%||
|PracticalWork.Library.Contracts.Events.BookCreatedEvent|100%||
|PracticalWork.Library.Contracts.Events.BookReturnedEvent|100%||
|PracticalWork.Library.Contracts.Events.ReaderClosedEvent|100%||
|PracticalWork.Library.Contracts.Events.ReaderCreatedEvent|100%||
|PracticalWork.Library.Contracts.Events.ReportCreateEvent|100%||
|PracticalWork.Library.Contracts.Exceptions.AppException|0%||
|PracticalWork.Library.Contracts.Exceptions.EntityNotFoundException`1|0%||
|PracticalWork.Library.Contracts.Exceptions.NotFoundException|0%||
|PracticalWork.Library.Contracts.Helpers.CacheKeyHasher|100%||
|PracticalWork.Library.Contracts.Helpers.CacheManager|92.4%|67.6%|
|PracticalWork.Library.Contracts.Models.ReportModels.ActivityLog|100%|100%|
|PracticalWork.Library.Contracts.Models.ReportModels.Report|100%||
|PracticalWork.Library.Contracts.v1.Abstracts.PaginationRequest|100%|50%|
|PracticalWork.Library.Contracts.v1.Books.Request.CreateBookRequest|0%||
|PracticalWork.Library.Contracts.v1.Books.Request.UpdateBookRequest|0%||
|PracticalWork.Library.Contracts.v1.Books.Response.BookDetailsResponse|0%||
|PracticalWork.Library.Contracts.v1.Books.Response.BookResponse|0%||
|PracticalWork.Library.Contracts.v1.Books.Response.BookWithIssuanceRecordsRe<br/>sponse|0%||
|PracticalWork.Library.Contracts.v1.Reader.Request.CreateReaderRequest|0%||

</details>
<details><summary>PracticalWork.Library.Data.Minio - 0%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**PracticalWork.Library.Data.Minio**|**0%**|**0%**|
|PracticalWork.Library.Data.Minio.Entry|0%||
|PracticalWork.Library.Data.Minio.MinioService|0%|0%|

</details>
<details><summary>PracticalWork.Library.Data.Reports.PostgreSql - 0%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**PracticalWork.Library.Data.Reports.PostgreSql**|**0%**|**0%**|
|PracticalWork.Library.Data.Reports.PostgreSql.Configurations.ActivityLogEnt<br/>ityConfiguration|0%||
|PracticalWork.Library.Data.Reports.PostgreSql.Configurations.EntityConfigur<br/>ationBase`1|0%||
|PracticalWork.Library.Data.Reports.PostgreSql.Configurations.ReportEntityCo<br/>nfiguration|0%||
|PracticalWork.Library.Data.Reports.PostgreSql.Entry|0%||
|PracticalWork.Library.Data.Reports.PostgreSql.Extensions.IQueryableExtensio<br/>ns|0%|0%|
|PracticalWork.Library.Data.Reports.PostgreSql.ReportsDbContext|0%|0%|
|PracticalWork.Library.Data.Reports.PostgreSql.ReportsDbContextFactory|0%|0%|
|PracticalWork.Library.Data.Reports.PostgreSql.ReportsMigrationsRunner|0%||
|PracticalWork.Library.Data.Reports.PostgreSql.Repositories.ActivityLogRepos<br/>itory|0%|0%|
|PracticalWork.Library.Data.Reports.PostgreSql.Repositories.ReportRepository|0%|0%|

</details>
<details><summary>PracticalWork.Library.MessageBroker - 4.9%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**PracticalWork.Library.MessageBroker**|**4.9%**|**0%**|
|PracticalWork.Library.MessageBroker.Entry|0%|0%|
|PracticalWork.Library.MessageBroker.Handlers.LibraryEventHandler|100%||
|PracticalWork.Library.MessageBroker.Options.RabbitMqOptions|0%||
|PracticalWork.Library.MessageBroker.Producer|0%||
|PracticalWork.Library.MessageBroker.Utils.RabbitMqInfrastructureInitializer|0%||
|PracticalWork.Library.MessageBroker.Workers.ConsumerWorker|0%|0%|
|PracticalWork.Library.MessageBroker.Workers.ProducerWorker|0%||

</details>
<details><summary>PracticalWork.Reports.Web - 34.4%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**PracticalWork.Reports.Web**|**34.4%**|**23.5%**|
|PracticalWork.Reports.Web.Configuration.DomainExceptionFilter`1|0%|0%|
|PracticalWork.Reports.Web.Configuration.ExceptionHandlingMiddleware|0%|0%|
|PracticalWork.Reports.Web.Controllers.ReportController|100%||
|PracticalWork.Reports.Web.MvcSetupExtensions|0%||
|PracticalWork.Reports.Web.Program|0%|0%|
|PracticalWork.Reports.Web.Services.ReportService|100%|75%|
|PracticalWork.Reports.Web.Startup|0%|0%|
|PracticalWork.Reports.Web.Validations.ActivityLogsPaginationRequestValidato<br/>r|100%|83.3%|
|PracticalWork.Reports.Web.Validations.ReportCreateRequestValidator|100%||

</details>
<details><summary>PracticalWork.Reports.Worker - 50.8%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**PracticalWork.Reports.Worker**|**50.8%**|**50%**|
|PracticalWork.Reports.Worker.Handlers.ReportEventHandler|100%||
|PracticalWork.Reports.Worker.ReportConsumerWorker|0%|0%|
|PracticalWork.Reports.Worker.Services.ReportGenerateService|100%|100%|
|PracticalWork.Reports.Worker.Services.ReportGenerationService|100%||
|Program|0%||

</details>
