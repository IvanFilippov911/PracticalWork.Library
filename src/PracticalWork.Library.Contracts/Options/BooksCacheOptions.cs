namespace PracticalWork.Library.Contracts.Options;

/// <summary>
/// Настройки кеша для книг и отчетов.
/// </summary>
public class BooksCacheOptions
{
    public CacheOptions BooksListCacheOptions { get; set; }

    public CacheOptions LibraryBooksCacheOptions { get; set; }

    public CacheOptions BookDetailsCacheOptions { get; set; }

    public CacheOptions ReportsCacheOptions { get; set; }
}
