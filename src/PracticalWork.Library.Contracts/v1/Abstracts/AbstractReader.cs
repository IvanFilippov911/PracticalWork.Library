namespace PracticalWork.Library.Contracts.v1.Abstracts;

/// <summary>
/// Абстрактная запись, представляющая базовую информацию о читателе
/// </summary>
/// <param name="FullName">Полное имя читателя</param>
/// <param name="PhoneNumber">Номер телефона читателя</param>
/// <param name="Email">Email адрес читателя</param>
/// <param name="ExpiryDate">Дата окончания действия читательского билета</param>
public abstract record AbstractReader(string FullName, string PhoneNumber, string Email, DateOnly ExpiryDate);
