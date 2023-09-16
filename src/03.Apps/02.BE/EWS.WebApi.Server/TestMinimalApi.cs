using EWS.Entity.Db;
using Microsoft.EntityFrameworkCore;

namespace EWS.WebApi.Server;

public class TestMinimalApi
{
    public WebApplication UseTodoApi(WebApplication app)
    {
        var mapGroup = app.MapGroup("users");
        mapGroup.MapGet("/", async (EWSMsDbContext db) => db.Users.FirstAsync())
            .CacheOutput(builder =>
            {
                builder.Cache()
                    .Expire(TimeSpan.FromSeconds(10));
            });
        return app;
    }
}