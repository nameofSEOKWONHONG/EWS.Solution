using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
// ReSharper disable InvalidXmlDocComment

namespace EWS.Domain.Infra.Mongodb;

/// <summary>
/// Example :
/// private readonly IMongoDbHandler _handler;
/// public Test(IMongoDbHandler handler)
/// {
///     _handler = handler;
/// }
/// 
/// public async Task Run()
/// {
///     var context = await _handler.Prepare("test",  "", "")
///         .ExecuteAsync<SessionContext>(async collection =>
///         {
///             var result = await collection.FindAsync(m => m.SessionId == "test");
///             return await result.FirstOrDefaultAsync();
///         });
/// }
/// </summary>
public sealed class MongoDbHandler : IMongoDbHandler
{
    private IMongoClient _client;
    private IMongoDatabase _db;
    private IConfiguration _configuration;
    private string _collectionName;

    public MongoDbHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public MongoDbHandler Prepare(string connection, string database, string collection)
    {
        _client = new MongoClient(connection.xIsNotEmpty() ? connection : _configuration.GetSection("").Value);
        _db = _client.GetDatabase(database.xIsNotEmpty() ? database : _configuration.GetSection("").Value);
        _collectionName = collection.xIsNotEmpty() ? collection : _configuration.GetSection("").Value;
        return this;
    }

    public async Task<T> ExecuteAsync<T>(Func<IMongoCollection<T>, Task<T>> func)
    {
        var collection = _db.GetCollection<T>(_collectionName);
        return await func(collection);
    }
}