using MongoDB.Driver;

namespace EWS.Domain.Infra.Mongodb;

public interface IMongoDbHandler
{
    MongoDbHandler Prepare(string connection, string database, string collection);
    Task<T> ExecuteAsync<T>(Func<IMongoCollection<T>, Task<T>> func);
}