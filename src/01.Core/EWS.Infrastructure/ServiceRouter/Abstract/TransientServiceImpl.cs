using EWS.Application.Service.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Infrastructure.ServiceRouter.Abstract;

public abstract class TransientServiceImpl<TSelf, TRequest, TResult> : ServiceImplBase<TSelf>,
    IServiceImplBase<TRequest, TResult>, ITransientService
{
    public TRequest Request { get; set; }
    public TResult Result { get; set; }
    public DbContext DbContext { get; set; }

    protected TransientServiceImpl(IHttpContextAccessor accessor) : base(accessor)
    {
        
    }
}