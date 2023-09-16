using System.Transactions;
using EWS.Domain.Base;
using EWS.Infrastructure.ServiceRouter.Abstract;
using eXtensionSharp;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;
using IsolationLevel = System.Data.IsolationLevel;

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
    private TransactionScopeOption _transactionScopeOption;
    private IsolationLevel _isolationLevel;
    
    #endregion

    #region [cache]

    private IDistributedCache _cache;
    private string _cacheKey;
    private DistributedCacheEntryOptions _cacheEntryOptions;    

    #endregion

    


    public ServiceLoader<TService, TRequest, TResult> UseTransaction<TDbContext>(TDbContext db, 
        IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
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
            IDbContextTransaction transaction = null;
            try
            {
                if (_db.Database.CurrentTransaction.xIsNotEmpty())
                {
                    transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted);
                }

                await ExecutedCore(resultAction);
                if (transaction.xIsNotEmpty()) await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "ServiceLoader OnExecuted Error : {Error}", e.Message);
                await transaction.RollbackAsync();
                throw;                
            }
        }
        else
        {
            await ExecutedCore(resultAction);
        }
    }
    

    private async Task ExecutedCore(Action<TResult> resultBehavior = null)
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
                if (_validateBehavior.xIsNotEmpty())
                {
                    _validateBehavior(validationResult);
                    return;
                }
            }
        }

        if (_cache.xIsNotEmpty())
        {
            var bytes = await _cache.GetAsync(_cacheKey.xGetHashCode());
            if (bytes.xIsNotEmpty())
            {
                var exist = bytes.xToString().xToEntity<TResult>();
                if (exist.xIsNotEmpty())
                {
                    _service.Result = exist;
                    return;
                }
            }    
        }
        
        
        var isOk = await _service.OnExecutingAsync();
        if (isOk)
        {
            await _service.OnExecuteAsync();
            if (resultBehavior.xIsNotEmpty())
            {
                resultBehavior(_service.Result);

                if (_cache.xIsNotEmpty())
                {
                    if (_service.Result.xIsNotEmpty())
                    {
                        await _cache.SetAsync(_cacheKey.xGetHashCode(), _service.Result.xToBytes(), _cacheEntryOptions);
                    }    
                }
            }
        }
    }
}