using Dapper;
using EWS.Application;
using EWS.Application.Const;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.Migrations;

public class MigrationUpExecuteService : IApplicationInitializeRunner
{
    private readonly DbContext _dbContext;
    public MigrationUpExecuteService(DbContext db)
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
        var connection = _dbContext.Database.GetDbConnection();
        foreach (var file in files.xToList())
        {
            if (file.xContains(new[]{".sql"}))
            {
                var sql = File.ReadAllText(file);
                connection.Execute(sql, null);
            }
        }
    }
}