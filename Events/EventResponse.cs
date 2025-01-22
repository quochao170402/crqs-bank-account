namespace Events;

public record EventResponse(Guid EventId, bool Success, string Message);
