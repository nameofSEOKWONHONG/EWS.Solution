using EWS.Application.Service.Abstract;
using EWS.Application.Wrapper;
using EWS.Domain.Base;
using EWS.Domain.Resource;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.Resource;

public interface IGetAllResourceService: IServiceImplBase<GetAllResourceRequest, JPaginatedResult<ResourceResult>>, IScopeService
{
    
}

public interface IGetResourceService : IServiceImplBase<JCodeRequestBase, IResultBase<Entity.Resource>>, IScopeService
{
    
}

public interface ISaveResourceService : IServiceImplBase<Entity.Resource, IResultBase<Entity.Resource>>, IScopeService
{
    
}

public interface IActiveResourceService : IServiceImplBase<Entity.Resource, IResultBase>, IScopeService
{
    
} 