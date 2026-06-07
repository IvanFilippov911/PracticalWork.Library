# Summary
<details open><summary>Summary</summary>

|||
|:---|:---|
| Generated on: | 26.05.2026 - 16:26:22 |
| Coverage date: | 26.05.2026 - 16:25:55 |
| Parser: | MultiReport (2x Cobertura) |
| Assemblies: | 4 |
| Classes: | 51 |
| Files: | 51 |
| **Line coverage:** | 79.9% (874 of 1093) |
| Covered lines: | 874 |
| Uncovered lines: | 219 |
| Coverable lines: | 1093 |
| Total lines: | 2353 |
| **Branch coverage:** | 75% (117 of 156) |
| Covered branches: | 117 |
| Total branches: | 156 |
| **Method coverage:** | [Feature is only available for sponsors](https://reportgenerator.io/pro) |

</details>

## Coverage
<details><summary>PracticalWork.Library - 77.5%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**PracticalWork.Library**|**77.5%**|**75%**|
|PracticalWork.Library.Abstractions.Storage.EntityBase|0%||
|PracticalWork.Library.Exceptions.AppException|66.6%||
|PracticalWork.Library.Exceptions.AuthenticationException|0%||
|PracticalWork.Library.Exceptions.BookServiceException|100%|50%|
|PracticalWork.Library.Exceptions.EntityNotFoundException`1|0%||
|PracticalWork.Library.Exceptions.LibraryServiceException|50%||
|PracticalWork.Library.Exceptions.NotFoundException|0%||
|PracticalWork.Library.Exceptions.ReaderServiceException|50%||
|PracticalWork.Library.Models.BookModels.Book|57.8%|50%|
|PracticalWork.Library.Models.BookModels.BookBorrow|100%|100%|
|PracticalWork.Library.Models.BookModels.BookCoverUpload|75%||
|PracticalWork.Library.Models.Common.PagedResult`1|0%|0%|
|PracticalWork.Library.Models.Common.PageRequest|100%|50%|
|PracticalWork.Library.Services.BookArchiveProcessingService|89.8%|80%|
|PracticalWork.Library.Services.BookService|65.1%|66.6%|
|PracticalWork.Library.Services.LibraryService|52.7%|60%|
|PracticalWork.Library.Services.ReaderService|79.1%|75%|
|PracticalWork.Library.Services.ReturnReminderProcessingService|100%|100%|
|PracticalWork.Library.Services.WeeklyAdminReportCsvBuilder|100%||
|PracticalWork.Library.Services.WeeklyAdminReportNotificationService|100%|100%|
|PracticalWork.Library.Services.WeeklyAdminReportPeriodService|100%||
|PracticalWork.Library.Services.WeeklyAdminReportProcessingService|100%||
|PracticalWork.Library.Services.WeeklyAdminReportStorageService|100%|100%|

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
<details><summary>PracticalWork.Reports.Web - 100%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**PracticalWork.Reports.Web**|**100%**|**75%**|
|PracticalWork.Reports.Web.Services.ReportService|100%|75%|

</details>
<details><summary>PracticalWork.Reports.Worker - 100%</summary>

|**Name**|**Line**|**Branch**|
|:---|---:|---:|
|**PracticalWork.Reports.Worker**|**100%**|**100%**|
|PracticalWork.Reports.Worker.Handlers.ReportEventHandler|100%||
|PracticalWork.Reports.Worker.Services.ReportGenerateService|100%|100%|
|PracticalWork.Reports.Worker.Services.ReportGenerationService|100%||

</details>
