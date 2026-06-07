namespace PracticalWork.Library.Contracts.Abstractions.MessageBroker;

/// <summary>
/// Обработчик для сообщений.
/// </summary>
public interface IMessageHandler<in TMessage>
{
    /// <summary>
    /// Обрабатывает входящее сообщение брокера.
    /// </summary>
    /// <param name="message">Сообщение для обработки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task HandleAsync(TMessage message, CancellationToken cancellationToken);
}
