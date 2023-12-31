﻿using EWS.Infrastructure.Session.Abstract;
using Microsoft.EntityFrameworkCore;

namespace EWS.Infrastructure.ServiceRouter.Abstract;

public interface IServiceImplBase
{
    Task<bool> OnExecutingAsync();
    Task OnExecuteAsync();
}

public interface IServiceImplBase<TRequest, TResult> : IServiceImplBase
{
    TRequest Request { get; set; }
    TResult Result { get; set; }
}