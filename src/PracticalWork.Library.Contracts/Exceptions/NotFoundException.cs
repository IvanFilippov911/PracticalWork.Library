namespace PracticalWork.Library.Contracts.Exceptions;

/// <summary>
/// Исключение для обозначения, что какие-то данные не найдены.
/// </summary>
public class NotFoundException : AppException
{
    public NotFoundException()
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}
