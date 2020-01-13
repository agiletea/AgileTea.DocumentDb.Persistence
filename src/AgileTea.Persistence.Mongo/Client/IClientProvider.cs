using System;
using MongoDB.Driver;

namespace AgileTea.Persistence.Mongo.Client
{
    /// <summary>
    /// Allows access to the Mongo Db Driver Client and Database in an abstracted manner.
    /// </summary>
    public interface IClientProvider : IDisposable
    {
        /// <summary>
        /// Return the Mongo Client.
        /// </summary>
        IMongoClient Client { get; }

        /// <summary>
        /// Returns the Database as per the options.
        /// </summary>
        IMongoDatabase Database { get; }
    }
}