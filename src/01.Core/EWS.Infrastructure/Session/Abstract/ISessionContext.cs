using EWS.Application.Language;
using EWS.Application.Session;
using Microsoft.Extensions.Configuration;

namespace EWS.Infrastructure.Session.Abstract;

public interface ISessionContext : ISessionContextBase
{
    ICurrentUserAccessor CurrentUserAccessor { get; set; }
    ICurrentTimeAccessor CurrentTimeAccessor { get; set; }
    IInfraAccessor InfraAccessor { get; set; }
    IConfiguration Configuration { get; set; }
    ILocalizer Localizer { get; set; }
}

