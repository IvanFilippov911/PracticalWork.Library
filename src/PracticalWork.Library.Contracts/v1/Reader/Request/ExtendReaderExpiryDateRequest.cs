namespace PracticalWork.Library.Contracts.v1.Reader.Request
{
    /// <summary>
    /// DTO запроса на продление срока действия читательского билета
    /// </summary>
    /// <param name="Date">Новая дата окончания действия читательского билета</param>
    public sealed record ExtendReaderExpiryDateRequest(DateOnly Date);
}
