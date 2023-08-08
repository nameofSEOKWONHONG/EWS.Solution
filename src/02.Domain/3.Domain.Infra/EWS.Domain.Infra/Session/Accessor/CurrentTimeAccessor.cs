using EWS.Infrastructure.Session.Abstract;

namespace EWS.Domain.Infra.Session.Accessor;

public class CurrentTimeAccessor : ICurrentTimeAccessor
{
    public DateTime Now
    {
        get
        {
            var utcDateTime = DateTime.UtcNow;
            var tenantTimeZone = TimeZoneInfo.FindSystemTimeZoneById(_currentUserAccessor.TimeZone);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tenantTimeZone);            
        }
    }
    public DateTime ToLocal(DateTime utc)
    {
        DateTime convertedDate = DateTime.SpecifyKind(
            utc,
            DateTimeKind.Utc);
        var kind = convertedDate.Kind; // will equal DateTimeKind.Utc
        if (kind != DateTimeKind.Utc) throw new Exception("param not utc datetime");
            
        return convertedDate.ToLocalTime();
    }

    public DateTime ToUtc(DateTime local) =>  local.ToUniversalTime();

    private readonly ICurrentUserAccessor _currentUserAccessor;
    public CurrentTimeAccessor(ICurrentUserAccessor currentUserAccessor)
    {
        _currentUserAccessor = currentUserAccessor;
    }
}