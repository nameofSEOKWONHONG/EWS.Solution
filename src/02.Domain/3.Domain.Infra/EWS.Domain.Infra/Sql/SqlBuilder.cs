using EWS.Application;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace EWS.Domain.Infra.Sql;

/// <summary>
/// Sqlkata query builder
/// 싱글톤으로 동작할 수 있음.
/// </summary>
public class SqlBuilder : DisposeBase
{
    private Func<Query> _query;

    private SqlBuilder()
    {
        
    }

    public SqlBuilder SetQuery(Func<Query> func)
    {
        _query = func;
        return this;
    }

    private readonly List<Query> _results = new List<Query>();
    public (QueryFactory dbFactory, Query query) Build(DbContext dbContext)
    {
        Query createdQuery = null;
        if (_query.xIsNotEmpty())
        {
            createdQuery = _query.Invoke();    
        }
        
        var db = new QueryFactory(dbContext.Database.GetDbConnection(), new SqlServerCompiler());
        #if DEBUG
        db.Logger = compiled =>
        {
            Log.Logger.Debug("sqlkata query : {Query}", compiled.ToString());
        };
        #endif
        return new(db, createdQuery);
    }


    public static SqlBuilder Create()
    {
        return new SqlBuilder();
    }
}