using PracticalWork.Library.Contracts.v1.Reader.Request;
using PracticalWork.Library.Models.ReaderModels;

namespace PracticalWork.Library.Controllers.Mappers.v1;

/// <summary>
/// Расширения для преобразования объектов, связанных с читателями
/// </summary>
public static class ReaderExtensions
{
    /// <summary>
    /// Преобразует объект запроса на создание читателя в модель Reader
    /// </summary>
    /// <param name="createReaderRequest">Объект запроса на создание читателя</param>
    /// <returns>Экземпляр модели Reader</returns>
    public static Reader ToReader(this CreateReaderRequest createReaderRequest) =>
        new()
        {
            FullName = createReaderRequest.FullName,
            PhoneNumber = createReaderRequest.PhoneNumber,
            Email = createReaderRequest.Email,
            ExpiryDate = createReaderRequest.ExpiryDate,
        };
}
