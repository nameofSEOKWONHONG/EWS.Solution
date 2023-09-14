using EWS.Application.Service.Abstract;
using EWS.Application.Wrapper;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.Users;

public interface IRemoveUserService : IServiceImplBase<string, IResultBase>, IScopeService
{
    
}