using EWS.Application.Wrapper;
using EWS.Entity;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.Users;

public interface IGetUserService : IServiceImplBase<string, IResultBase<User>>
{
    
}