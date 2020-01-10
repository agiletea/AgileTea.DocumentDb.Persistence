using System;
using MongoDB.Driver;

namespace AgileTea.Persistence.Mongo.Client
{
    public interface IClientProvider : IDisposable
    {
        IMongoClient Client { get; }
        IMongoDatabase Database { get; }
    }
}