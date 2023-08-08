using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.Extension;

public static class CastExtensions
{
    public static T vDbCast<T>(this DbContext dbContext)
        where T : DbContext
    {
        return dbContext as T;
    }
}