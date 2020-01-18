using MongoDB.Driver;

namespace AgileTea.Persistence.Mongo.Client
{
    internal interface IMongoDbClient : IMongoClient
    {
        bool CanSupportTransactions { get; }
    }
}
