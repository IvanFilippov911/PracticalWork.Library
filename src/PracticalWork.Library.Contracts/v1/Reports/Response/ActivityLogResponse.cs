using PracticalWork.Library.Contracts.Events;

namespace PracticalWork.Library.Contracts.v1.Reports.Response;

/// <summary>
/// Запись события системы
/// </summary>
/// <param name="Event">Объект события</param>
/// <param name="EventType">Тип события</param>
/// <param name="EventDate">Дата события</param>
public record ActivityLogResponse(BaseEvent Event, string EventType, DateTime EventDate);