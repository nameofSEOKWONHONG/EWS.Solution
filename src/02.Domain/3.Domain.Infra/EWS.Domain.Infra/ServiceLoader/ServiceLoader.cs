using EWS.Domain.Base;
using EWS.Infrastructure.ServiceRouter.Abstract;
using eXtensionSharp;
using FluentValidation.Results;

namespace EWS.Domain.Infrastructure;

public class ServiceLoader<TService, TRequest, TResult>
where TService : IServiceImplBase<TRequest, TResult>
{
    private IServiceImplBase<TRequest, TResult> _service;
    private ServiceLoader(IServiceImplBase<TRequest, TResult> service)
    {
        _service = service;
    }

    public static ServiceLoader<TService, TRequest, TResult> Create(IServiceImplBase<TRequest, TResult> service)
    {
        return new ServiceLoader<TService, TRequest, TResult>(service);
    }

    private List<Func<bool>> _filters = new List<Func<bool>>();
    private Func<TRequest> _parameter;
    private JValidatorBase<TRequest> _validator; 

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

    public async Task OnExecuted(Action<TResult, ValidationResult> action)
    {
        var filterValid = true;
        _filters.xForEach(filter =>
        {
            filterValid = filter.Invoke();
            if (filterValid.xIsFalse()) return false;
            return true;
        });

        if (filterValid.xIsFalse()) return;

        TRequest parameter = default(TRequest);
        if (_parameter.xIsNotEmpty())
        {
            parameter = _parameter.Invoke();
        }

        var serviceImpl = (ServiceImplBase<TService>)_service;
        if (_validator.xIsNotEmpty())
        {
            var validationResult = await _validator.ValidateAsync(parameter);
            if (validationResult.IsValid.xIsFalse())
            {
                action(default, validationResult);
                return;
            }            
        }
        
        var isOk = await serviceImpl.OnExecutingAsync();
        if (isOk)
        {
            await serviceImpl.OnExecuteAsync();
            var serviceImplInterface = (IServiceImplBase<TRequest, TResult>)_service;
            action(serviceImplInterface.Result, null);
        }
    }
}