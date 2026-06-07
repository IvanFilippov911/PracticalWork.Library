namespace PracticalWork.Library.Models.NotificationModels;

/// <summary>
/// Результат отправки email.
/// </summary>
public sealed record EmailSendResult(bool Success, string ErrorMessage = "");
