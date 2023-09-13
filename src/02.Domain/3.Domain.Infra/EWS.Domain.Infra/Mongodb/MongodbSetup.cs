using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace EWS.Domain.Infra.Mongodb;

public static class MongoDbSetupExtensions
{
    public static IServiceCollection vAddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("BookStoreDatabase:ConnectionString").Value;
        var database = configuration.GetSection("BookStoreDatabase:DatabaseName").Value;
        var collection = configuration.GetSection("BookStoreDatabase:BooksCollectionName").Value;
        services.AddSingleton<IMongoClient>(new MongoClient(config));
        services.AddScoped<IMongoDatabase>(provider =>
        {
            var client = provider.GetService<IMongoClient>();
            return client.GetDatabase(database);
        });

        services.AddScoped<IMongoDbHandler, MongoDbHandler>();

        return services;
    }
}