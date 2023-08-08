using Microsoft.Extensions.DependencyInjection;

namespace EWS.Infrastructure.ServiceRouter.Abstract;

public interface IServiceProviderRegister
{
    void RegisterService(IServiceCollection services);
}