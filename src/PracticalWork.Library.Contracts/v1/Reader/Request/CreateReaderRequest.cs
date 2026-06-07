using PracticalWork.Library.Contracts.v1.Abstracts;

namespace PracticalWork.Library.Contracts.v1.Reader.Request
{
    /// <summary>
    /// Запрос на создание нового читателя
    /// </summary>
    /// <param name="FullName">Полное имя читателя</param>
    /// <param name="PhoneNumber">Номер телефона читателя</param>
    /// <param name="Email">Email адрес читателя</param>
    /// <param name="ExpiryDate">Дата окончания действия читательского билета</param>
    public sealed record CreateReaderRequest(string FullName, string PhoneNumber, string Email, DateOnly ExpiryDate) 
        : AbstractReader(FullName, PhoneNumber, Email, ExpiryDate);
}
