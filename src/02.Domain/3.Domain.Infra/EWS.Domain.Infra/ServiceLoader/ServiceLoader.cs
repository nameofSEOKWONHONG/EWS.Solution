using EWS.Domain.Base;
using EWS.Infrastructure.ServiceRouter.Abstract;
using eXtensionSharp;
using FluentValidation.Results;

namespace EWS.Domain.Infrastructure;

public static class ServiceLoaderExtensions {
    public static ServiceLoader<TService, TRequest, TResult> Create<TService, TRequest, TResult>(this IServiceImplBase<TRequest, TResult> service)
        where TService : IServiceImplBase<TRequest, TResult>
    {
        return new ServiceLoader<TService, TRequest, TResult>(service);
    }
}


public sealed class ServiceLoader<TService, TRequest, TResult>
where TService : IServiceImplBase<TRequest, TResult>
{
    private IServiceImplBase<TRequest, TResult> _service;
    internal ServiceLoader(IServiceImplBase<TRequest, TResult> service)
    {
        _service = service;
    }

    private List<Func<bool>> _filters = new List<Func<bool>>();
    private Func<TRequest> _parameter;
    private JValidatorBase<TRequest> _validator;
    private Func<ValidationResult, Task> _validateResultFunc;
    private Func<TResult, Task> _resultFunc;

    public ServiceLoader<TService, TRequest, TResult> AddFilter(Func<bool> filter)
    {
        _filters.Add(filter);
        return this;
    }

    public ServiceLoader<TService, TRequest, TResult> SetParameter(Func<TRequest> parameter)
    {
        _parameter = parameter;
        return this;
    }

    public ServiceLoader<TService, TRequest, TResult> SetValidator(JValidatorBase<TRequest> validator)
    {
        _validator = validator;
        return this;
    }

    public ServiceLoader<TService, TRequest, TResult> OnValidated(Func<ValidationResult, Task> validationResultFunc)
    {
        _validateResultFunc = validationResultFunc;
        return this;
    }

    public async Task OnExecuted(Func<TResult, Task> resultAction = null)
    {   
        var filterValid = true;
        _filters.ForEach(filter =>
        {
            filterValid = filter.Invoke();
            if (filterValid.xIsFalse()) return;
        });

        if (filterValid.xIsFalse()) return;

        TRequest parameter = default(TRequest);
        if (_parameter.xIsNotEmpty())
        {
            parameter = _parameter.Invoke();
            _service.Request = parameter;
        }

        if (_validator.xIsNotEmpty())
        {
            var validationResult = await _validator.ValidateAsync(parameter);
            if (validationResult.IsValid.xIsFalse())
            {
                if (_validateResultFunc.xIsNotEmpty())
                {
                    await _validateResultFunc(validationResult);
                    return;
                }
            }            
        }
        
        var service = (IServiceImplBase<TRequest, TResult>)_service;
        var isOk = await service.OnExecutingAsync();
        if (isOk)
        {
            await service.OnExecuteAsync();
            if (resultAction.xIsNotEmpty())
            {
                await resultAction(service.Result);    
            }
        }
    }
}