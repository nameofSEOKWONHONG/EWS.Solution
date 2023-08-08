using EWS.Application.Wrapper;
using EWS.Domain.Identity;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.Account.Identity;

public interface IResetPasswordService : IServiceImplBase<ResetPasswordRequest, IResultBase>
{
    
}