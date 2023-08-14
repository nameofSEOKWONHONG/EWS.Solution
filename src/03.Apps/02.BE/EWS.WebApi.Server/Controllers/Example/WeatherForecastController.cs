using System.Transactions;
using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.WeatherForecast;
using EWS.Domain.Base;
using EWS.Domain.Example;
using EWS.Domain.Implement.Example.BackgroundJob.Abstract;
using EWS.Entity.Db;
using EWS.Domain.Infra;
using EWS.Entity.Example;
using EWS.Infrastructure.Extentions;
using EWS.Infrastructure.ServiceRouter.Implement.Routers;
using eXtensionSharp;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace EWS.WebApi.Server.Controllers;

/// <summary>
/// 
/// </summary>
public class WeatherForecastController : JControllerBase
{
    private readonly IBackgroundJobClient _client;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="accessor"></param>
    /// <param name="client"></param>
    public WeatherForecastController(IHttpContextAccessor accessor, IBackgroundJobClient client) : base(accessor)
    {
        _client = client;
    }

    [HttpGet("getall")]
    public async Task<JPaginatedResult<WeatherForecastResult>> GetAll([FromQuery]WeatherForecatGetAllRequest request)
    {
        var result = await this.CreateServiceRouter<EWSMsDbContext, IWeatherForecastGetAllService, WeatherForecatGetAllRequest,
            JPaginatedResult<WeatherForecastResult>>(request);
        _client.Enqueue<IMyBackgroundJob>(job => job.Run());
        return result;
    }

    [HttpGet(Name = "get")]
    public async Task<WeatherForecastResult> Get(int id)
    {   
        WeatherForecastResult result = null;
        using var sr = ServiceRouter.Create<EWSMsDbContext>(this.Accessor, TransactionScopeOption.Suppress);
        var now = DateTime.Now;
        sr.Register<IWeatherForecastGetService, int, WeatherForecastResult>()
            .AddFilter(() => true)
            .SetParameter(() => id)
            .Executed(res =>
            {
                result = res;
            });

        await sr.ExecuteAsync();

        return result;
    }
    
    [HttpPost(Name = "add")]
    public async Task<IActionResult> Add(WeatherForecast request)
    {
        IResultBase<int> result = null;
        using (var sr = ServiceRouter.Create<EWSMsDbContext>(this.Accessor, TransactionScopeOption.Required))
        {
            sr.Register<IWeatherForecastAddService, WeatherForecast, IResultBase<int>>()
                .AddFilter(() => request.Id <= 0)
                .SetParameter(() => request)
                .Executed((res) => result = res);
            await sr.ExecuteAsync();
        }

        return Ok(result);
    }

    // [HttpPost(Name = "GetBatch")]
    // public async Task<IEnumerable<WeatherForecastResult>> GetBatch(int[] ids)
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



    // [HttpGet("GetSingle")]
    // public async Task<IActionResult> GetSingle([FromQuery]int i)
    // {
    //     WeatherForecast result = null;
    //     using (var sr = ServiceRouter.Create<ExampleJDbContext>(_accessor, TransactionScopeOption.Required))
    //     {
    //         sr.Register<IWeatherForecastSingletonService, int, WeatherForecast>()
    //             .AddFilter(() => i > 0)
    //             .SetParameter(() => i)
    //             .Executed((res) => result = res);
    //         await sr.ExecuteAsync();
    //     }
    //
    //     return Ok(result);
    // }

    [HttpGet("CallWorker")]
    public async Task<IActionResult> CallWorker(string connectionId)
    {
        var result = new List<WeatherForecastResult>();
        using (var sr = ServiceRouter.Create<EWSMsDbContext>(this.Accessor, TransactionScopeOption.Suppress))
        {
            sr.Register<IWeatherForecastServiceV2, string, WeatherForecastResult>()
                .AddFilter(() => connectionId.xIsNotEmpty())
                .SetParameter(() => connectionId)
                .Executed(res =>
                {
                    result.Add(res);
                });
            await sr.ExecuteAsync();
        }

        return Ok(await JResult<List<WeatherForecastResult>>.SuccessAsync(result));
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> Bulk()
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
        
        using (var sr = ServiceRouter.Create<EWSMsDbContext>(this.Accessor, TransactionScopeOption.Required))
        {
            sr.Register<IWeatherForecastBulkInsertService, IEnumerable<WeatherForecastBulkRequest>, IResultBase>()
                .AddFilter(() => list.Count > 0)
                .SetParameter(() => list)
                .Executed((res) => result = res);
            await sr.ExecuteAsync();
        }

        return Ok(result);
    }
}