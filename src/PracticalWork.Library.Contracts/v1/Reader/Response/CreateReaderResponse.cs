namespace PracticalWork.Library.Contracts.v1.Reader.Response
{
    /// <summary>
    /// DTO ответа после успешного создания читателя
    /// </summary>
    /// <param name="Id">Идентификатор созданного читателя</param>
    public sealed record CreateReaderResponse(Guid Id);
}
