﻿using System.Collections.Concurrent;
using System.Reflection;
using EWS.Domain.Base;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
using Dapper;
using EWS.Infrastructure.ObjectPool;

namespace EWS.Domain.Infra.Sql;

public static class SqlBulkBuilderExtensions
{
    public static SqlBulkBuilder<T> CreateSqlBulkBuilder<T>(this DbContext db)
        where T : JBulkBase
    {
        return new SqlBulkBuilder<T>(db);
    }
}

public class SqlBulkBuilder<T>
    where T : JBulkBase
{
    private readonly DbContext _dbContext;
    private const int BATCH_SIZE = 500;
    private const int BATCH_LIMIT = 2000;

    internal SqlBulkBuilder(DbContext db)
    {
        _dbContext = db;
    }

    public async Task BulkInsertAsync(string schema, string tableName, T[] items)
    {
        if (items.xIsEmpty()) return;
        
        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
            item.CreatedOn = item.CreatedOn.AddMilliseconds(i + 1);
        }
        
        var batchItems = items.xBatch(BATCH_SIZE);
        foreach (T[] batchItem in batchItems)
        {
            var sql = CreateSql(schema, tableName, batchItem);
            //await this._dbContext.Database.ExecuteSqlRawAsync(sql);
            await this._dbContext.Database.GetDbConnection().ExecuteAsync(sql);
            await Task.Delay(100);
        }
    }

    public Task BulkInsertAsync<TEntity>(T[] items)
    {
        var assignSchemaAndTable = AssignSchemaAndTable();
        return this.BulkInsertAsync(assignSchemaAndTable.schemaName, assignSchemaAndTable.tableName, items);
    }
    
    private string CreateSql(string schema, string tableName, T[] items)
    {
        var columns = AssignColumn(items.First());
        var datum = AssignDatum(columns, items);

        var sql = $$"""
                  SET NOCOUNT ON;
                  
                  INSERT INTO {{schema}}.{{tableName}} ({{columns.xJoin()}})
                  VALUES {{datum.xJoin()}}
                  
                  SET NOCOUNT OFF;  
                  """;

        return sql;
    }

    #region [function]

    private static readonly ConcurrentDictionary<Type, string[]> _assignColumnStates = new();

    private string[] AssignColumn(T item)
    {
        Type itemType = typeof(T);
        if (_assignColumnStates.TryGetValue(itemType, out string[] cachedColumns))
        {
            return cachedColumns;
        }
        var columns = new List<string>();
        var props = item.xGetProperties();
        props.xForEach(prop =>
        {
            columns.Add(prop.Name);
            return true;
        });
        _assignColumnStates.TryAdd(itemType, columns.ToArray());
        return columns.ToArray();
    }
    
    private static readonly ConcurrentDictionary<Type, (string schemaName, string tableName)> AssignTableStates = new();

    private (string schemaName, string tableName) AssignSchemaAndTable()
    {
        Type itemType = typeof(T);
        if (AssignTableStates.TryGetValue(itemType, out (string schemaName, string tableName) cachedResult))
        {
            return cachedResult;
        }
        var result = XAttributeExtensions.xGetSchemaAndTableName<T>();
        AssignTableStates.TryAdd(itemType, result);
        return result;
    }    
    
    private static readonly Dictionary<string, Func<PropertyInfo, object, string>> DatumStates = new()
    {
        {
            //s += $"DECLARE @{i}{j} NVARCHAR(MAX) = '{item2.GetValue(item)}'" + Environment.NewLine;    
            DataTypeName.String, (prop, item) => $"N'{prop.GetValue(item).xValue<string>()}',"
        },
        {
            DataTypeName.DateTime, (prop, item) =>
            {
                var v = prop.GetValue(item).xValue<DateTime>();
                if (v.xIsEmpty()) return $"NULL, ";
                return $"N'{v.ToString(ENUM_DATE_FORMAT.YYYY_MM_DD_HH_MM_SS_FFF)}', ";
            }
        },
        {
            DataTypeName.NullableDateTime, (prop, item) =>
            {
                var v = prop.GetValue(item).xValue<DateTime?>();
                if (v.xIsEmpty()) return $"NULL, ";
                return $"N'{v!.Value}', ";
            }
        },
        {
            DataTypeName.Boolean, (prop, item) => $"{prop.GetValue(item).xValue<int>(0)}, "
        },
        {
            "Default", (prop, item) => $"{prop.GetValue(item).xValue<string>("0")}, "
        }
    };    
    
    private IEnumerable<string> AssignDatum(string[] columns, T[] items)
    {
        var sbPool = StringBuilderPool.Create(items.Length);
        
        var result = new List<string>();
        items.xForEach((item, i) =>
        {
            var props = item.xGetProperties();
            var statement = sbPool.Rent();
            statement.Append("(");            
            props.xForEach((prop, j) =>
            {
                var exist = columns.xFirst(m => m == prop.Name);
                if (exist.xIsEmpty()) return true;

                // ReSharper disable once AccessToModifiedClosure
                statement.Append(
                    DatumStates.TryGetValue(prop.PropertyType.ToString(), out Func<PropertyInfo, object, string> x)
                        ? x(prop, item)
                        : DatumStates["Default"](prop, item));
                return true;
            });
            var valueSql = statement.ToString();
            valueSql = valueSql.Substring(0, valueSql.LastIndexOf(','));
            if (i < items.Count() - 1)
            {
                valueSql += " ) " + Environment.NewLine;
            }
            else
            {
                valueSql += " ); " + Environment.NewLine;
            }
            result.Add(valueSql);
        });
        return result.ToArray();
    }         

    #endregion

}

