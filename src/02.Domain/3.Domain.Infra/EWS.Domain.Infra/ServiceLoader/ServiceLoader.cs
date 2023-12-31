using System.Transactions;
using EWS.Domain.Base;
using EWS.Infrastructure.ServiceRouter.Abstract;
using eXtensionSharp;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace EWS.Domain.Infrastructure;

public static class ServiceLoaderExtensions {
    public static ServiceLoader<TService, TRequest, TResult> Create<TService, TRequest, TResult>(this TService service)
        where TService : IServiceImplBase<TRequest, TResult>
    {
        // ReSharper disable once HeapView.PossibleBoxingAllocation
        return new ServiceLoader<TService, TRequest, TResult>(service);
    }
}

public sealed class ServiceLoader<TService, TRequest, TResult>
where TService : IServiceImplBase<TRequest, TResult>
{
    private readonly IServiceImplBase<TRequest, TResult> _service;
    internal ServiceLoader(IServiceImplBase<TRequest, TResult> service)
    {
        _service = service;
    }

    #region [action behavior's]

    private List<Func<bool>> _filters = new List<Func<bool>>();
    private Func<TRequest> _parameter;
    private JValidatorBase<TRequest> _validator;
    private Action<ValidationResult> _validateBehavior;
    private Func<TResult, Task> _resultFunc;    

    #endregion

    #region [database transaction]
    
    private DbContext _db;
    private bool _useTransaction;
    private bool _useAdoTransaction;
    private TransactionScopeOption _transactionScopeOption;
    private System.Transactions.IsolationLevel _isolationLevel;
    
    #endregion

    #region [cache]

    private IDistributedCache _cache;
    private string _cacheKey;
    private DistributedCacheEntryOptions _cacheEntryOptions;

    #endregion


    public ServiceLoader<TService, TRequest, TResult> UseTransaction<TDbContext>(TDbContext db, 
        TransactionScopeOption option = TransactionScopeOption.Required,
        System.Transactions.IsolationLevel isolationLevel = System.Transactions.IsolationLevel.ReadUncommitted)
    where TDbContext : DbContext
    {
        _db = db;
        _useTransaction = true;
        _isolationLevel = isolationLevel;
        return this;
    }
    
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

    public ServiceLoader<TService, TRequest, TResult> OnValidated(Action<ValidationResult> validateBehavior)
    {
        _validateBehavior = validateBehavior;
        return this;
    }

    public ServiceLoader<TService, TRequest, TResult> SetOutputCache(IDistributedCache cache, string key, DistributedCacheEntryOptions options = null)
    {
        if (cache.xIsEmpty()) throw new Exception("Cache is empty.");
        if (key.xIsEmpty()) throw new Exception("Cache key is empty.");
        
        _cache = cache;
        _cacheKey = key;
        _cacheEntryOptions = options;
        if (_cacheEntryOptions.xIsEmpty()) _cacheEntryOptions = new DistributedCacheEntryOptions();

        _cacheKey = $"{_cacheKey}|{typeof(TService).Name}";
        
        return this;
    }

    public async Task OnExecuted(Action<TResult> resultAction = null)
    {
        if (_useTransaction.xIsTrue())
        {
            using var scope = new TransactionScope(_transactionScopeOption,
                new TransactionOptions() { IsolationLevel = _isolationLevel },
                TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                await ExecuteCore(resultAction);
                scope.Complete();
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "ServiceLoader OnExecuted Error : {Error}", e.Message);
                throw;                    
            }
        }
        else
        {
            await ExecuteCore(resultAction);
        }
    }

    #region [execute core]
    private async Task ExecuteCore(Action<TResult> resultBehavior = null)
    {
        if(InvokedFilter().xIsFalse()) return;

        InvokedParameter();

        var valid = await InvokedValidatingAsync(_service.Request);
        if(valid.xIsFalse()) return;

        await GetCacheAsync();

        await ExecuteAsync(resultBehavior);

        await SetCacheAsync();
    }
    
    private bool InvokedFilter()
    {
        var filterValid = true;
        _filters.ForEach(filter =>
        {
            filterValid = filter.Invoke();
            if (filterValid.xIsFalse()) return;
        });

        return filterValid;
    }

    private void InvokedParameter()
    {
        TRequest parameter = default(TRequest);

        if (_parameter.xIsNotEmpty())
        {
            parameter = _parameter.Invoke();
            _service.Request = parameter;
        }
    }

    private async Task<bool> InvokedValidatingAsync(TRequest parameter)
    {
        if (_validator.xIsNotEmpty())
        {
            var rs = await _validator.ValidateAsync(parameter);
            if (rs.IsValid.xIsFalse())
            {
                _validateBehavior(rs);
            }

            return rs.IsValid;
        }

        return true;
    }

    private async Task GetCacheAsync()
    {
        if (_cache.xIsNotEmpty())
        {
            var bytes = await _cache.GetAsync(_cacheKey.xGetHashCode());
            if (bytes.xIsNotEmpty())
            {
                var exist = bytes.xToString().xToEntity<TResult>();
                if (exist.xIsNotEmpty())
                {
                    _service.Result = exist;
                }
            }    
        }
    }

    private async Task ExecuteAsync(Action<TResult> resultBehavior)
    {
        var isOk = await _service.OnExecutingAsync();
        if (isOk)
        {
            await _service.OnExecuteAsync();
            if (resultBehavior.xIsNotEmpty())
            {
                resultBehavior(_service.Result);
            }
        }
    }

    private async Task SetCacheAsync()
    {
        if (_cache.xIsNotEmpty())
        {
            if (_service.Result.xIsNotEmpty())
            {
                await _cache.SetAsync(_cacheKey.xGetHashCode(), _service.Result.xToBytes(), _cacheEntryOptions);
            }    
        }
    }    

    #endregion
}