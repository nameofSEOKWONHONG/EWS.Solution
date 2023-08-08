using Dapper;
using EWS.Entity;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.Service;

public class SequentialService : ISequentialService
{
    public SequentialService()
    {
        
    }
    
    public async Task<int> ExecuteAsync(DbContext db, ISessionContext context, Sequential request)
    {
        return await db.Database.GetDbConnection().ExecuteScalarAsync<int>(
            "EXEC [DBO].[PROC_GET_NEXT_SEQUENTIAL] @TENANT_ID, @TABLE_NAME, @YEAR, @MONTH, @DAY", new
            {
                TENANT_ID = context.TenantId,
                TABLE_NAME = request.TableName,
                YEAR = request.Year,
                MONTH = request.Month,
                DAY = request.Day
            });
    }
}


