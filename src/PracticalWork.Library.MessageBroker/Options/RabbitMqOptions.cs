namespace PracticalWork.Library.MessageBroker.Options;

/// <summary>
/// Настройки подключения и конфигурации RabbitMQ
/// </summary>
public class RabbitMqOptions
{
    /// <summary>
    /// Хост RabbitMQ
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Пользователь для подключения
    /// </summary>
    public string User { get; set; } = "guest";

    /// <summary>
    /// Пароль для подключения
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// Порт RabbitMQ
    /// </summary>
    public int? Port { get; set; } = 5672;

    /// <summary>
    /// Название приложения
    /// </summary>
    public string AppName { get; set; } = "library";

    /// <summary>
    /// Настройки обмена и очередей для библиотеки
    /// </summary>
    public LibraryOptions Library { get; set; } = new();

    /// <summary>
    /// Настройки обмена и очередей для отчетов
    /// </summary>
    public ReportsOptions Reports { get; set; } = new();
}

/// <summary>
/// Настройки обмена и очередей для библиотеки
/// </summary>
public class LibraryOptions
{
    /// <summary>
    /// Название обмена для библиотеки
    /// </summary>
    public string ExchangeName { get; set; } = "library.exchange";

    /// <summary>
    /// Настройки очереди и ключа маршрутизации для создания книги
    /// </summary>
    public BookCreateOptions BookCreate { get; set; } = new();

    /// <summary>
    /// Настройки очереди и ключа маршрутизации для архивирования книги
    /// </summary>
    public BookArchiveOptions BookArchive { get; set; } = new();

    /// <summary>
    /// Настройки очереди и ключа маршрутизации для выдачи книги
    /// </summary>
    public BookBorrowOptions BookBorrow { get; set; } = new();

    /// <summary>
    /// Настройки очереди и ключа маршрутизации для возврата книги
    /// </summary>
    public BookReturnOptions BookReturn { get; set; } = new();

    /// <summary>
    /// Настройки очереди и ключа маршрутизации для создания читателя
    /// </summary>
    public ReaderCreateOptions ReaderCreate { get; set; } = new();

    /// <summary>
    /// Настройки очереди и ключа маршрутизации для закрытия читателя
    /// </summary>
    public ReaderCloseOptions ReaderClose { get; set; } = new();
}

/// <summary>
/// Настройки очереди и ключа маршрутизации для создания книги
/// </summary>
public class BookCreateOptions
{
    /// <summary>
    /// Название очереди для обработки создания книги
    /// </summary>
    public string QueueName { get; set; } = "book.create.queue";

    /// <summary>
    /// Ключ маршрутизации для сообщений о создании книги
    /// </summary>
    public string RoutingKey { get; set; } = "book.create.key";
}

/// <summary>
/// Настройки очереди и ключа маршрутизации для архивирования книги
/// </summary>
public class BookArchiveOptions
{
    /// <summary>
    /// Название очереди для обработки архивирования книги
    /// </summary>
    public string QueueName { get; set; } = "book.archive.queue";

    /// <summary>
    /// Ключ маршрутизации для сообщений об архивировании книги
    /// </summary>
    public string RoutingKey { get; set; } = "book.archive.key";
}

/// <summary>
/// Настройки очереди и ключа маршрутизации для выдачи книги
/// </summary>
public class BookBorrowOptions
{
    /// <summary>
    /// Название очереди для обработки выдачи книги
    /// </summary>
    public string QueueName { get; set; } = "book.borrow.queue";

    /// <summary>
    /// Ключ маршрутизации для сообщений о выдаче книги
    /// </summary>
    public string RoutingKey { get; set; } = "book.borrow.key";
}

/// <summary>
/// Настройки очереди и ключа маршрутизации для возврата книги
/// </summary>
public class BookReturnOptions
{
    /// <summary>
    /// Название очереди для обработки возврата книги
    /// </summary>
    public string QueueName { get; set; } = "book.return.queue";

    /// <summary>
    /// Ключ маршрутизации для сообщений о возврате книги
    /// </summary>
    public string RoutingKey { get; set; } = "book.return.key";
}

/// <summary>
/// Настройки очереди и ключа маршрутизации для создания читателя
/// </summary>
public class ReaderCreateOptions
{
    /// <summary>
    /// Название очереди для обработки создания читателя
    /// </summary>
    public string QueueName { get; set; } = "reader.create.queue";

    /// <summary>
    /// Ключ маршрутизации для сообщений о создании читателя
    /// </summary>
    public string RoutingKey { get; set; } = "reader.create.key";
}

/// <summary>
/// Настройки очереди и ключа маршрутизации для закрытия читателя
/// </summary>
public class ReaderCloseOptions
{
    /// <summary>
    /// Название очереди для обработки закрытия читателя
    /// </summary>
    public string QueueName { get; set; } = "reader.close.queue";

    /// <summary>
    /// Ключ маршрутизации для сообщений о закрытии читателя
    /// </summary>
    public string RoutingKey { get; set; } = "reader.close.key";
}

/// <summary>
/// Настройки очереди и обмена для отчетов
/// </summary>
public class ReportsOptions
{
    /// <summary>
    /// Название очереди отчетов
    /// </summary>
    public string QueueName { get; set; } = "reports.queue";

    /// <summary>
    /// Название обмена отчетов
    /// </summary>
    public string Exchange { get; set; } = "reports.exchange";

    /// <summary>
    /// Ключ маршрутизации для создания отчетов
    /// </summary>
    public string RoutingKey { get; set; } = "reports.create.key";
}
