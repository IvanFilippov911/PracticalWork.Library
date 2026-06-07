namespace PracticalWork.Library.Options
{
    /// <summary>
    /// Настройки кеша для книг
    /// </summary>
    public class BooksCacheOptions
    {
        /// <summary>
        /// Настройки кеша для списка книг с фильтрацией и пагинацией (books:list:{hash})
        /// </summary>
        public CacheOptions BooksListCacheOptions { get; set; }
        
        /// <summary>
        /// Настройки кеша для списка книг модуля работы библиотеки (library:books:{hash})
        /// </summary>
        public CacheOptions LibraryBooksCacheOptions { get; set; }
        
        /// <summary>
        /// Настройки кеша для деталей книг (book:details:{id})
        /// </summary>
        public CacheOptions BookDetailsCacheOptions { get; set; }

        public CacheOptions ReportsCacheOptions { get; set; }
    }
}