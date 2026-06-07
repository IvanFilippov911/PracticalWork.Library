using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Models.NotificationModels;
using PracticalWork.Library.Options;

namespace PracticalWork.Library.Services.Email;

/// <summary>
/// Сборщик email для напоминания о возврате книги.
/// </summary>
public sealed class ReturnReminderEmailBuilder : IReturnReminderEmailBuilder
{
    private readonly EmailTemplateRenderer _templateRenderer;

    public ReturnReminderEmailBuilder(EmailTemplateRenderer templateRenderer)
    {
        _templateRenderer = templateRenderer;
    }

    public async Task<EmailMessage> BuildAsync(
        ReturnReminderCandidate candidate,
        ReturnReminderTemplate template,
        CancellationToken cancellationToken = default)
    {
        var placeholders = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["ReaderFullName"] = candidate.ReaderFullName,
            ["BookTitle"] = candidate.BookTitle,
            ["BookAuthors"] = string.Join(", ", candidate.BookAuthors),
            ["DueDate"] = candidate.DueDate.ToString("dd.MM.yyyy"),
            ["DaysLeft"] = template.DaysBeforeDueDate.ToString(),
            ["LibraryAddress"] = template.LibraryAddress,
            ["LibraryPhone"] = template.LibraryPhone,
            ["LibraryHours"] = template.WorkingHours
        };

        var subject = template.SubjectTemplate
            .Replace("{BookTitle}", candidate.BookTitle, StringComparison.Ordinal);

        var htmlBody = await _templateRenderer
            .RenderAsync("return-reminder.html", placeholders, cancellationToken);
        var textBody = await _templateRenderer
            .RenderAsync("return-reminder.txt", placeholders, cancellationToken);

        return new EmailMessage
        {
            To = candidate.ReaderEmail,
            Subject = subject,
            HtmlBody = htmlBody,
            TextBody = textBody
        };
    }
}
