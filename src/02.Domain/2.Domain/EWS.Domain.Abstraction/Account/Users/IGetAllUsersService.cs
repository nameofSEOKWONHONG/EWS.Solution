using System.Collections.Generic;
using EWS.Application.Wrapper;
using EWS.Domain.Account;
using EWS.Domain.Account.Users;
using EWS.Domain.Base;
using EWS.Entity;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.Users;

public interface IGetAllUsersService : IServiceImplBase<GetAllUsersRequest, JPaginatedResult<UserResult>>
{
    
}