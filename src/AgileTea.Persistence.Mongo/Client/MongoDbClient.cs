using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;

namespace AgileTea.Persistence.Mongo.Client
{
    [ExcludeFromCodeCoverage]
    internal sealed class MongoDbClient : MongoClient, IMongoDbClient
    {
        public MongoDbClient(string dbConnection, bool canSupportTransactions)
            : base(dbConnection)
        {
            CanSupportTransactions = canSupportTransactions;
        }

        public bool CanSupportTransactions { get; }
    }
}
