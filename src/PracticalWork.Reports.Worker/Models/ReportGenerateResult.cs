namespace PracticalWork.Reports.Worker.Models;

/// <summary>
/// Результат генерации файла отчета.
/// </summary>
public class ReportGenerateResult
{
    /// <summary>
    /// Содержимое файла отчета.
    /// </summary>
    public required Stream Content { get; set; }

    /// <summary>
    /// MIME-тип содержимого файла.
    /// </summary>
    public required string ContentType { get; set; }

    /// <summary>
    /// Имя файла отчета в хранилище.
    /// </summary>
    public required string FileName { get; set; }
}
