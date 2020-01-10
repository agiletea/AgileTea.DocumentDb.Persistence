using AgileTea.Persistence.Common.Contexts;
using MongoDB.Driver;

namespace AgileTea.Persistence.Mongo.Context
{
    public interface IMongoContext : IDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
