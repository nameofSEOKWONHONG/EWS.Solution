using System.Text;
using Dapper;
using EWS.Application;
using EWS.Application.Const;
using EWS.Infrastructure.ObjectPool;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.Migrations;

public class MigrationDownExecuteService : IApplicationInitializeRunner
{
    private readonly DbContext _dbContext;
    public MigrationDownExecuteService(DbContext db)
    {
        _dbContext = db;
    }
    
    public bool Filter(string[] args)
    {
        return true;
    }

    public void Execute(string[] args)
    {
        var migPath = Path.Combine(MigrationConst.WEBSERVICE_PATH, "MIGRATION");
        var files = Directory.GetFiles(migPath);
        var sb = StringBuilderPool.Shared.Rent();
        var connection = _dbContext.Database.GetDbConnection();
        foreach (var file in files.xToList())
        {
            if (file.xContains(".sql"))
            {
                var sqls = File.ReadAllLines(file);
                var isRead = false;
                for (var i = 0; i < sqls.Length; i++)
                {
                    var line = sqls[i];
                    isRead = line.xContains("-- RESTORE");
                    if (isRead)
                    {
                        for (var i1 = i+1; i1 < sqls.Length; i1++)
                        {
                            var readLine = sqls[i1];
                            if (readLine.xContains("-- RESTORE"))
                            {
                                break;
                            }
                            else
                            {
                                sb.AppendLine(readLine);
                            }
                        }                        
                    }
                    if(isRead.xIsFalse()) continue;
                    break;
                }
            }
            connection.Execute(sb.ToString(), null);            
            sb.Clear();
        }
    }
}