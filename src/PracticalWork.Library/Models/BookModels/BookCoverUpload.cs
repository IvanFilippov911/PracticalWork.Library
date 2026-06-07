namespace PracticalWork.Library.Models.BookModels;

/// <summary>
/// Содержимое загруженной обложки книги.
/// </summary>
public sealed class BookCoverUpload
{
    public static BookCoverUpload Empty { get; } = new(null, null, null);

    public BookCoverUpload(string fileName, string contentType, Stream stream)
    {
        FileName = fileName;
        ContentType = contentType;
        Stream = stream;
    }

    /// <summary>
    /// Имя файла.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// MIME-тип файла.
    /// </summary>
    public string ContentType { get; }

    /// <summary>
    /// Поток содержимого файла.
    /// </summary>
    public Stream Stream { get; }

    /// <summary>
    /// Признак наличия файла.
    /// </summary>
    public bool HasFile => Stream is not null;
}
