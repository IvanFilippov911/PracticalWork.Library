namespace PracticalWork.Library.Models.NotificationModels;

/// <summary>
/// Данные email сообщения.
/// </summary>
public sealed class EmailMessage
{
    /// <summary>Адрес получателя.</summary>
    public required string To { get; init; }

    /// <summary>Тема письма.</summary>
    public required string Subject { get; init; }

    /// <summary>HTML-версия письма.</summary>
    public string HtmlBody { get; init; } = string.Empty;

    /// <summary>Текстовая версия письма.</summary>
    public string TextBody { get; init; } = string.Empty;
}
