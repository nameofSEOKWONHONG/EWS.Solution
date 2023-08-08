namespace EWS.Infrastructure.Session.Abstract;

public interface ICurrentTimeAccessor
{
    DateTime Now { get; }
    DateTime ToLocal(DateTime utc);
    DateTime ToUtc(DateTime local);
}