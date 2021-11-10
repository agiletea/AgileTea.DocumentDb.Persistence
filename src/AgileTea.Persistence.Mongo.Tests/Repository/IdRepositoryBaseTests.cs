using System;
using System.Threading;
using System.Threading.Tasks;
using AgileTea.Persistence.Common.Interfaces;
using AgileTea.Persistence.Mongo.Context;
using AgileTea.Persistence.Mongo.Repository;
using AgileTea.Persistence.Mongo.Tests.Helpers;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace AgileTea.Persistence.Mongo.Tests.Repository
{
    public abstract class IdRepositoryBaseTests<TId, TIdDocumentRepository, TIdDocument>
        where TIdDocument : class, IIndexedEntity<TId>, new()
        where TIdDocumentRepository : DocumentRepositoryBase<TIdDocument, TId>
        where TId : new()
    {
        private readonly ILogger<TIdDocumentRepository> logger = Mock.Of<ILogger<TIdDocumentRepository>>();
        private readonly IMongoContext context = Mock.Of<IMongoContext>();
        private readonly ILoggerFactory loggerFactory = Mock.Of<ILoggerFactory>();

        protected IdRepositoryBaseTests()
        {
            Mock.Get(LoggerFactory)
                .Setup(x => x.CreateLogger(typeof(TIdDocumentRepository).FullName))
                .Returns(logger);
        }

        protected IMongoContext Context => context;
        protected ILoggerFactory LoggerFactory => loggerFactory;

        protected abstract TId Id { get; }

        protected abstract string ExpectedJsonIdFilter { get; }

        [Fact]
        public void GivenADocumentRepository_WhenAddIsCalled_CollectionIsTakenFromContextAndInsertIsCalled()
        {
            // arrange
            var target = CreateRepository();
            var expected = new TIdDocument();
            var testCollection = Mock.Of<IMongoCollection<TIdDocument>>();

            Mock.Get(Context)
                .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
                .Returns(testCollection)
                .Verifiable();

            Mock.Get(Context)
                .Setup(x => x.AddCommand(It.IsAny<Func<Task>>()))
                .Callback(async (Func<Task> func) => { await func.Invoke().ConfigureAwait(false); })
                .Verifiable();

            Mock.Get(testCollection)
                .Setup(x => x.InsertOneAsync(expected, null, default))
                .Returns(Task.FromResult(testCollection))
                .Verifiable();

            // act
            target.Add(expected);

            // assert
            Mock.Verify(Mock.Get(Context));
            Mock.Verify(Mock.Get(testCollection));
        }

        [Fact]
        public void GivenADocumentRepository_WhenUpdateIsCalled_CollectionIsTakenFromContextAndReplaceOneIsCalled()
        {
            // arrange
            var target = CreateRepository();
            var expected = Mock.Of<TIdDocument>();

            Mock.Get(expected).Setup(x => x.Id).Returns(Id);

            var testCollection = Mock.Of<IMongoCollection<TIdDocument>>();

            Mock.Get(Context)
                .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
                .Returns(testCollection)
                .Verifiable();

            Mock.Get(Context)
                .Setup(x => x.AddCommand(It.IsAny<Func<Task>>()))
                .Callback(async (Func<Task> func) => { await func.Invoke().ConfigureAwait(false); })
                .Verifiable();

            Mock.Get(testCollection)
                .Setup(x => x.ReplaceOneAsync(
                    It.Is<FilterDefinition<TIdDocument>>(filter => filter.RenderToJson().Equals(ExpectedJsonIdFilter)),
                    expected,
                    It.IsAny<ReplaceOptions>(),
                    default))
                .Returns(Task.FromResult((ReplaceOneResult)new ReplaceOneResult.Acknowledged(1L, 1L, new BsonInt64(1L))))
                .Verifiable();

            // act
            target.Update(expected);

            // assert
            Mock.Verify(Mock.Get(Context));
            Mock.Verify(Mock.Get(testCollection));
        }

        [Fact]
        public void GivenADocumentRepository_WhenRemoveIsCalled_CollectionIsTakenFromContextAndDeleteOneIsCalled()
        {
            // arrange
            var target = CreateRepository();
            var testCollection = Mock.Of<IMongoCollection<TIdDocument>>();

            Mock.Get(Context)
                .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
                .Returns(testCollection)
                .Verifiable();

            Mock.Get(Context)
                .Setup(x => x.AddCommand(It.IsAny<Func<Task>>()))
                .Callback(async (Func<Task> func) => { await func.Invoke().ConfigureAwait(false); })
                .Verifiable();

            Mock.Get(testCollection)
                .Setup(x => x.DeleteOneAsync(
                    It.Is<FilterDefinition<TIdDocument>>(filter => filter.RenderToJson().Equals(ExpectedJsonIdFilter)),
                    default))
                .Returns(Task.FromResult((DeleteResult)new DeleteResult.Acknowledged(1L)))
                .Verifiable();

            // act
            target.Remove(Id);

            // assert
            Mock.Verify(Mock.Get(Context));
            Mock.Verify(Mock.Get(testCollection));
        }

        [Fact]
        public async Task GivenADocumentRepository_WhenGetAllAsyncIsCalled_CollectionIsTakenFromContext()
        {
            // arrange
            var target = CreateRepository();
            var testCollection = Mock.Of<IMongoCollection<TIdDocument>>();
            var findCollection = Mock.Of<IAsyncCursor<TIdDocument>>();
            const string expectedJsonFilter = "{ }";
            int index = 0;
            var expected = new[] { new TIdDocument(), new TIdDocument() };

            Mock.Get(Context)
                .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
                .Returns(testCollection)
                .Verifiable();

            Mock.Get(findCollection)
                .Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
                .Callback((CancellationToken ct) =>
                {
                    index++;
                })
                .Returns(() => index < expected.Length);

            Mock.Get(findCollection)
                .Setup(x => x.Current)
                .Returns(expected);

            Mock.Get(testCollection)
                .Setup(x => x.FindAsync(
                    It.Is<FilterDefinition<TIdDocument>>(filter => filter.RenderToJson().Equals(expectedJsonFilter)),
                    It.IsAny<FindOptions<TIdDocument, TIdDocument>>(),
                    default))
                .Returns(Task.FromResult(findCollection))
                .Verifiable();

            // act
            var actual = await target.GetAllAsync().ConfigureAwait(false);

            // assert
            Assert.Equal(expected, actual);
            Mock.Verify(Mock.Get(Context));
            Mock.Verify(Mock.Get(testCollection));
        }

        [Fact]
        public void GivenADocumentRepository_WhenGetAllIsCalled_CollectionIsTakenFromContext()
        {
            // arrange
            var target = CreateRepository();
            var testCollection = Mock.Of<IMongoCollection<TIdDocument>>();
            var findCollection = Mock.Of<IAsyncCursor<TIdDocument>>();
            const string expectedJsonFilter = "{ }";
            int index = 0;
            var expected = new[] { new TIdDocument(), new TIdDocument() };

            Mock.Get(Context)
                .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
                .Returns(testCollection)
                .Verifiable();

            Mock.Get(findCollection)
                .Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
                .Callback((CancellationToken ct) =>
                {
                    index++;
                })
                .Returns(() => index < expected.Length);

            Mock.Get(findCollection)
                .Setup(x => x.Current)
                .Returns(expected);

            Mock.Get(testCollection)
                .Setup(x => x.FindSync(
                    It.Is<FilterDefinition<TIdDocument>>(filter => filter.RenderToJson().Equals(expectedJsonFilter)),
                    It.IsAny<FindOptions<TIdDocument, TIdDocument>>(),
                    default))
                .Returns(findCollection)
                .Verifiable();

            // act
            var actual = target.GetAll();

            // assert
            Assert.Equal(expected, actual);
            Mock.Verify(Mock.Get(Context));
            Mock.Verify(Mock.Get(testCollection));
        }

        [Fact]
        public async Task GivenADocumentRepository_WhenGetByIdAsyncIsCalled_CollectionIsTakenFromContext()
        {
            // arrange
            var target = CreateRepository();
            var testCollection = Mock.Of<IMongoCollection<TIdDocument>>();
            var findCollection = Mock.Of<IAsyncCursor<TIdDocument>>();
            var expected = new TIdDocument();
            Mock.Get(Context)
                .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
                .Returns(testCollection)
                .Verifiable();

            Mock.Get(findCollection)
                .Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true);

            Mock.Get(findCollection)
                .Setup(x => x.Current)
                .Returns(new[] { expected });

            Mock.Get(testCollection)
                .Setup(x => x.FindAsync(
                    It.Is<FilterDefinition<TIdDocument>>(filter => filter.RenderToJson().Equals(ExpectedJsonIdFilter)),
                    It.IsAny<FindOptions<TIdDocument, TIdDocument>>(),
                    default))
                .Returns(Task.FromResult(findCollection))
                .Verifiable();

            // act
            var actual = await target.GetByIdAsync(Id).ConfigureAwait(false);

            // assert
            Assert.Equal(expected, actual);
            Mock.Verify(Mock.Get(Context));
            Mock.Verify(Mock.Get(testCollection));
        }

        [Fact]
        public void GivenADocumentRepository_WhenGetByIdIsCalled_CollectionIsTakenFromContext()
        {
            // arrange
            var target = CreateRepository();
            var testCollection = Mock.Of<IMongoCollection<TIdDocument>>();
            var findCollection = Mock.Of<IAsyncCursor<TIdDocument>>();
            var expected = new TIdDocument();
            Mock.Get(Context)
                .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
                .Returns(testCollection)
                .Verifiable();

            Mock.Get(findCollection)
                .Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true);

            Mock.Get(findCollection)
                .Setup(x => x.Current)
                .Returns(new[] { expected });

            Mock.Get(testCollection)
                .Setup(x => x.FindSync(
                    It.Is<FilterDefinition<TIdDocument>>(filter => filter.RenderToJson().Equals(ExpectedJsonIdFilter)),
                    It.IsAny<FindOptions<TIdDocument, TIdDocument>>(),
                    default))
                .Returns(findCollection)
                .Verifiable();

            // act
            var actual = target.GetById(Id);

            // assert
            Assert.Equal(expected, actual);
            Mock.Verify(Mock.Get(Context));
            Mock.Verify(Mock.Get(testCollection));
        }

        [Fact]
        public async Task GivenAnErrorGettingTheCollection_WhenGetAllIsCalled_ExceptionIsThrown()
        {
            // arrange
            var target = CreateRepository();
            Mock.Get(Context)
                .Setup(x => x.GetCollection<TIdDocument>(target.CollectionName))
                .Throws(new Exception("Test error message"))
                .Verifiable();

            // act
            var actual = await Assert.ThrowsAsync<Exception>(async () =>
                    await target.GetAllAsync().ConfigureAwait(false))
                .ConfigureAwait(false);

            // assert
            Assert.Equal("Test error message", actual.Message);
            Mock.Verify(Mock.Get(Context));
        }

        protected abstract TIdDocumentRepository CreateRepository();
    }
}
