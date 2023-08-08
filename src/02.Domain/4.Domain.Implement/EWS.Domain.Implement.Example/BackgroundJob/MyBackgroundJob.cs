using EWS.Application.Service.Abstract;
using EWS.Domain.Implement.Example.BackgroundJob.Abstract;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Example.BackgroundJob;

public class MyBackgroundJob : IMyBackgroundJob, IScopeService
{
    private readonly EWSMsDbContext _dbContext;
    public MyBackgroundJob(EWSMsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Run()
    {
        var result = await _dbContext.WeatherForecasts.Take(10).ToListAsync();
        result.ForEach(item =>
        {
            Console.WriteLine(item.Id);
        });
    }
}