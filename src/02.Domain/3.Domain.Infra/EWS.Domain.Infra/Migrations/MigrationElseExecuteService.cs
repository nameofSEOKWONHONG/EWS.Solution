using Dapper;
using EWS.Application;
using EWS.Application.Const;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.Migrations;

public class MigrationElseExecuteService : IApplicationInitializeRunner
{
    private readonly DbContext _dbContext;
    public MigrationElseExecuteService(DbContext db)
    {
        _dbContext = db;
    }
    
    private void ExecuteUpdateFunction()
    {
        var path = Path.Combine(MigrationConst.WEBSERVICE_PATH, "Function");
        var files = Directory.GetFiles(path);
        if (files.xIsNotEmpty())
        {
            var connection = _dbContext.Database.GetDbConnection();
            foreach (var file in files.xToList())
            {
                if (file.xContains(new[]{"[DBO].", ".sql"}))
                {
                    var sql = File.ReadAllText(file);
                    connection.Execute(sql, null);
                }
            }
        }
    }

    private void ExecuteUpdateProcedure()
    {
        var path = Path.Combine(MigrationConst.WEBSERVICE_PATH, "Procedure");
        var dirs = Directory.GetDirectories(path);
        if (dirs.xIsNotEmpty())
        {
            var connection = _dbContext.Database.GetDbConnection();
            foreach (var dir in dirs)
            {
                var files = Directory.GetFiles(dir);
                if (files.xIsNotEmpty())
                {
                    foreach (var file in files.xToList())
                    {
                        if (file.xContains(new[]{"[DBO].", ".sql"}))
                        {
                            var sql = File.ReadAllText(file);
                            connection.Execute(sql, null);
                        }
                    }                    
                }
            }
        }
    }

    public bool Filter(string[] args)
    {
        return true;
    }

    public void Execute(string[] args)
    {
        ExecuteUpdateFunction();
        ExecuteUpdateProcedure();
    }
}