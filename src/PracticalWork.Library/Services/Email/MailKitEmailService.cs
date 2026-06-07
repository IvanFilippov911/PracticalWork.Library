using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Models.NotificationModels;
using PracticalWork.Library.Options;

namespace PracticalWork.Library.Services.Email;

/// <inheritdoc />
public sealed class MailKitEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<MailKitEmailService> _logger;

    public MailKitEmailService(
        IOptions<EmailSettings> settings,
        ILogger<MailKitEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<EmailSendResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            mimeMessage.To.Add(MailboxAddress.Parse(message.To));
            mimeMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message.HtmlBody,
                TextBody = message.TextBody
            };
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(
                _settings.SmtpServer,
                _settings.SmtpPort,
                _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                cancellationToken);

            await smtpClient.SendAsync(mimeMessage, cancellationToken);
            await smtpClient.DisconnectAsync(true, cancellationToken);

            return new EmailSendResult(true);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Ошибка отправки email на {Receiver}", message.To);
            return new EmailSendResult(false, exception.Message);
        }
    }
}
