using System;
using System.Threading;
using System.Threading.Tasks;
using AgileTea.Persistence.Common.Entities;
using AgileTea.Persistence.Mongo.Client;
using AgileTea.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace AgileTea.Persistence.Mongo.Tests.Context
{
    public class MongoContextTests
    {
        private readonly IClientProvider clientProvider = Mock.Of<IClientProvider>();
        private readonly ILoggerFactory loggerFactory = Mock.Of<ILoggerFactory>();
        private readonly ILogger<MongoContext> logger = Mock.Of<ILogger<MongoContext>>();
        private readonly IMongoClient client = Mock.Of<IMongoClient>();
        private readonly IMongoDatabase database = Mock.Of<IMongoDatabase>();

        public MongoContextTests()
        {
            Mock.Get(loggerFactory).Setup(x => x.CreateLogger(typeof(MongoContext).FullName)).Returns(logger);
            Mock.Get(clientProvider).Setup(x => x.Client).Returns(client);
            Mock.Get(clientProvider).Setup(x => x.Database).Returns(database);
        }

        [Fact]
        public async Task GivenClientIsNull_WhenSaveChangesCalled_InvalidOperationExceptionIsThrown()
        {
            // arrange
            var target = new MongoContext(clientProvider, loggerFactory);
            Mock.Get(clientProvider).Setup(x => x.Client).Returns(null as IMongoClient);

            // act/ assert
            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => target.SaveChangesAsync());

            // assert
            Assert.Equal("Mongo check failed. Client is null", result.Message);
        }

        [Fact]
        public async Task GivenDatabaseIsNull_WhenSaveChangesCalled_InvalidOperationExceptionIsThrown()
        {
            // arrange
            var target = new MongoContext(clientProvider, loggerFactory);
            Mock.Get(clientProvider).Setup(x => x.Database).Returns(null as IMongoDatabase);

            // act/ assert
            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => target.SaveChangesAsync());

            // assert
            Assert.Equal("Mongo check failed. Database is null", result.Message);
        }

        [Fact]
        public void GivenClientIsNull_WhenGetCollectionCalled_InvalidOperationExceptionIsThrown()
        {
            // arrange
            var target = new MongoContext(clientProvider, loggerFactory);
            Mock.Get(clientProvider).Setup(x => x.Client).Returns(null as IMongoClient);

            // act/ assert
            var result = Assert.Throws<InvalidOperationException>(() => target.GetCollection<TestDocument>(typeof(TestDocument).Name));

            // assert
            Assert.Equal("Mongo check failed. Client is null", result.Message);
        }

        [Fact]
        public void GivenDatabaseIsNull_WhenGetCollectionCalled_InvalidOperationExceptionIsThrown()
        {
            // arrange
            var target = new MongoContext(clientProvider, loggerFactory);
            Mock.Get(clientProvider).Setup(x => x.Database).Returns(null as IMongoDatabase);

            // act/ assert
            var result = Assert.Throws<InvalidOperationException>(() => target.GetCollection<TestDocument>(typeof(TestDocument).Name));

            // assert
            Assert.Equal("Mongo check failed. Database is null", result.Message);
        }

        [Fact]
        public void GivenMongoClientAndDatabase_WhenCollectionRequested_DatabaseGetCollectionIsCalled()
        {
            // arrange
            var target = new MongoContext(clientProvider, loggerFactory);
            var expected = Mock.Of<IMongoCollection<TestDocument>>();

            Mock.Get(database)
                .Setup(x => x.GetCollection<TestDocument>(typeof(TestDocument).FullName, null))
                .Returns(expected)
                .Verifiable();

            // act
            var actual = target.GetCollection<TestDocument>(typeof(TestDocument).FullName);

            // assert
            Mock.Verify(Mock.Get(database));
            Assert.Equal(expected, actual);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Code Smell",
            "S2699:Tests should include assertions",
            Justification = "Just a check to ensure that if no writes are made then, with the session being null, " +
                            "calling Dispose on it within the Dispose method of MongoContext does not throw an Exception")]
        public void GivenANoActiveSession_WhenDisposed_NoExceptionIsThrown()
        {
            // arrange
            var target = new MongoContext(clientProvider, loggerFactory);

            // act and assert - by not calling the SaveChanges method, no mongo session is created and therefore will be null
            target.Dispose();
        }

        [Fact]
        public async Task GivenAnActiveSession_WhenDisposed_TheSessionIsDisposed()
        {
            // arrange
            var target = new MongoContext(clientProvider, loggerFactory);
            var sessionHandle = Mock.Of<IClientSessionHandle>();

            Mock.Get(client)
                .Setup(x => x.StartSessionAsync(null, default(CancellationToken)))
                .ReturnsAsync(sessionHandle)
                .Verifiable();

            // no commands set up so nothing to test inside of this method beyond the creation of the session
            await target.SaveChangesAsync();

            // act
            target.Dispose();

            // assert
            Mock.Get(sessionHandle).Verify(x => x.Dispose());
            Mock.Verify(Mock.Get(client));
        }

        [Fact]
        public async Task GivenCommands_WhenSaveChangesCalled_TheCommandsAreActioned()
        {
            // arrange
            var target = new MongoContext(clientProvider, loggerFactory);
            var sessionHandle = Mock.Of<IClientSessionHandle>();

            Mock.Get(sessionHandle)
                .Setup(x => x.StartTransaction(null))
                .Verifiable();

            Mock.Get(sessionHandle)
                .Setup(x => x.CommitTransactionAsync(default(CancellationToken)))
                .Returns(Task.FromResult(0))
                .Verifiable();

            Mock.Get(client)
                .Setup(x => x.StartSessionAsync(null, default(CancellationToken)))
                .ReturnsAsync(sessionHandle)
                .Verifiable();

            bool commandHasRun = false;

            target.AddCommand(() => Task.Run(() => commandHasRun = true));

            // act
            await target.SaveChangesAsync();

            // assert
            Assert.True(commandHasRun);
            Mock.Verify(Mock.Get(sessionHandle));
        }

        public class TestDocument : IndexedEntityBase
        {
        }
    }
}
