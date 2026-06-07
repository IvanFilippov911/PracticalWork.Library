namespace PracticalWork.Library.Options;

/// <summary>
/// Настройки архивации книг.
/// </summary>
public class ArchiveSettings
{
    public int YearsWithoutBorrow { get; set; } = 3;

    public int MaxBooksPerRun { get; set; } = 100;
}
