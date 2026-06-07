using System.Text.RegularExpressions;

namespace PracticalWork.Library.Jobs.Common;

/// <summary>
/// Базовая проверка корректности cron-выражения (5 полей).
/// </summary>
public static class CronExpressionValidator
{
    private static readonly Regex FieldPattern = new(
        "^[0-9*/,-]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static bool TryValidate(string expression, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(expression))
        {
            error = "CronExpression не может быть пустым.";
            return false;
        }

        var parts = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 5)
        {
            error = "CronExpression должен содержать 5 полей: minute hour day month dayOfWeek.";
            return false;
        }

        for (var i = 0; i < parts.Length; i++)
        {
            if (!FieldPattern.IsMatch(parts[i]))
            {
                error = $"Поле cron #{i + 1} содержит недопустимые символы: '{parts[i]}'";
                return false;
            }
        }

        return true;
    }
}
