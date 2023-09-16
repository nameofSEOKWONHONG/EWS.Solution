using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Base;
using EWS.Domain.Example;
using EWS.Domain.Implement.Example.BackgroundJob.Abstract;
using EWS.Domain.Infra;
using EWS.Domain.Infrastructure;
using EWS.Entity.Db;
using EWS.Entity.Example;
using EWS.Infrastructure.Extentions;
using eXtensionSharp;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace EWS.WebApi.Server.Controllers;

/// <summary>
/// 
/// </summary>
public class WeatherForecastController : JUnverifiedControllerBase<EWSMsDbContext>
{
    private readonly IBackgroundJobClient _client;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="accessor"></param>
    /// <param name="client"></param>
    public WeatherForecastController() : base()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("getall")]
    public async Task<JPaginatedResult<WeatherForecastResult>> GetAll([FromServices]IWeatherForecastGetAllService service,
        [FromServices]IBackgroundJobClient backgroundJobClient,
        [FromQuery]WeatherForecatGetAllRequest request)
    {   
        JPaginatedResult<WeatherForecastResult> result = null;
        await service
            .Create<IWeatherForecastGetAllService, WeatherForecatGetAllRequest,
                JPaginatedResult<WeatherForecastResult>>()
            .AddFilter(request.xIsNotEmpty)
            .SetParameter(() => request)
            .OnExecuted(v => result = result);
        
        backgroundJobClient.Enqueue<IMyBackgroundJob>(job => job.Run());
        
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet(Name = "get")]
    public async Task<WeatherForecastResult> Get([FromServices]IWeatherForecastGetService service,
        int id)
    {   
        WeatherForecastResult result = null;
        await service.Create<IWeatherForecastGetService, int, WeatherForecastResult>()
            .AddFilter(() => id.xIsNotEmptyNum())
            .SetParameter(() => id)
            .OnExecuted(r => result = r);
        
        return result;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost(Name = "add")]
    public async Task<IActionResult> Add([FromServices]IWeatherForecastAddService service,
        WeatherForecast request)
    {
        IResultBase<int> result = null;
        await service.Create<IWeatherForecastAddService, WeatherForecast, IResultBase<int>>()
            .UseTransaction(this.Db)
            .AddFilter(() => request.Id.xIsNotEmptyNum())
            .SetParameter(() => request)
            .OnExecuted((res) => result = res);
        
        return Ok(result);
    }

    // [HttpPost(Name = "batch")]
    // public async Task<IEnumerable<WeatherForecastResult>> Batch(int[] ids)
    // {
    //     var result = new List<WeatherForecastResult>();
    //     using (var batch = ServiceBatchRouter.Create<ExampleJDbContext, int>(_accessor, TransactionScopeOption.Suppress, ids))
    //     {
    //         batch.Register<IWeatherForecastServiceV2, DateOnly, WeatherForecastResult>()
    //             .AddFilter(current => true)
    //             .SetParameter(current => current)
    //             .Executed(res =>
    //             {
    //                 result.Add(res);
    //             });
    //         await batch.ExecuteAsync();
    //     }
    //
    //     return result;
    // }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    [HttpGet("callworker")]
    public async Task<IActionResult> CallWorker([FromServices]IWeatherForecastServiceV2 service, string connectionId)
    {
        var result = new List<WeatherForecastResult>();
        await service.Create<IWeatherForecastServiceV2, string, WeatherForecastResult>()
            .AddFilter(() => connectionId.xIsNotEmpty())
            .SetParameter(() => connectionId)
            .OnExecuted(res =>
            {
                result.Add(res);
            });

        return Ok(await JResult<List<WeatherForecastResult>>.SuccessAsync(result));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPost("bulk")]
    public async Task<IActionResult> Bulk([FromServices]IWeatherForecastBulkInsertService service)
    {
        var cities = new[]
        {
            "seoul", "busan", "gangwon", "inchon", "guyungki"
        };
        IResultBase result = null;
        var list = new List<WeatherForecastBulkRequest>();
        Enumerable.Range(1, 2000).ToList().ForEach(i =>
        {
            list.Add(new WeatherForecastBulkRequest()
            {
                TenantId = "00000",
                Id = i + 1,
                City = cities[Random.Shared.Next(0, 4)],
                Date = DateTime.Now.AddSeconds(Random.Shared.Next(1, 59)),
                TemperatureC = Random.Shared.Next(1, 59),
                Summary = Random.Shared.Next(1, 1000).ToString(),
                WeatherForecastType = Random.Shared.Next(0, 3),
                CreatedBy = "TEST",
                CreatedName = "TEST".vToAESEncrypt(),
                CreatedOn = DateTime.Now.AddSeconds(Random.Shared.Next(1, 59)),
                IsActive = true
            });
        });
        
        await service.Create<IWeatherForecastBulkInsertService, IEnumerable<WeatherForecastBulkRequest>, IResultBase>()
            .UseTransaction(this.Db)
            .AddFilter(() => list.Count.xIsNotEmptyNum())
            .SetParameter(() => list)
            .OnExecuted((res) => result = res);
        
        return Ok(result);
    }
}