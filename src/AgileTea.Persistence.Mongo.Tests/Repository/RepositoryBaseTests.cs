using System;
using System.Threading;
using System.Threading.Tasks;
using AgileTea.Persistence.Common.Entities;
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
    public class RepositoryBaseTests
    {
        private readonly IMongoContext context = Mock.Of<IMongoContext>();
        private readonly ILoggerFactory loggerFactory = Mock.Of<ILoggerFactory>();
        private readonly ILogger<TestDocumentRepository> logger = Mock.Of<ILogger<TestDocumentRepository>>();

        public RepositoryBaseTests()
        {
            Mock.Get(loggerFactory)
                .Setup(x => x.CreateLogger(typeof(TestDocumentRepository).FullName))
                .Returns(logger);
        }

        [Fact]
        public void GivenADocumentRepository_WhenAddIsCalled_CollectionIsTakenFromContextAndInsertIsCalled()
        {
            // arrange
            var target = new TestDocumentRepository(context, logger);
            var expected = new TestDocument();
            var testCollection = Mock.Of<IMongoCollection<TestDocument>>();

            Mock.Get(context)
                .Setup(x => x.GetCollection<TestDocument>(typeof(TestDocument).Name))
                .Returns(testCollection)
                .Verifiable();

            Mock.Get(context)
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
            Mock.Verify(Mock.Get(context));
            Mock.Verify(Mock.Get(testCollection));
        }

        [Fact]
        public void GivenADocumentRepository_WhenUpdateIsCalled_CollectionIsTakenFromContextAndReplaceOneIsCalled()
        {
            // arrange
            var target = new TestDocumentRepository(context, logger);
            var id = Guid.NewGuid();
            var expectedJsonFilter = "{ \"_id\" : CSUUID(\"" + id + "\") }";
            var expected = new TestDocument
            {
                Id = id
            };
            var testCollection = Mock.Of<IMongoCollection<TestDocument>>();

            Mock.Get(context)
                .Setup(x => x.GetCollection<TestDocument>(typeof(TestDocument).Name))
                .Returns(testCollection)
                .Verifiable();

            Mock.Get(context)
                .Setup(x => x.AddCommand(It.IsAny<Func<Task>>()))
                .Callback(async (Func<Task> func) => { await func.Invoke().ConfigureAwait(false); })
                .Verifiable();

            Mock.Get(testCollection)
                .Setup(x => x.ReplaceOneAsync(
                    It.Is<FilterDefinition<TestDocument>>(filter => filter.RenderToJson().Equals(expectedJsonFilter)),
                    expected,
                    It.IsAny<ReplaceOptions>(),
                    default))
                .Returns(Task.FromResult((ReplaceOneResult)new ReplaceOneResult.Acknowledged(1L, 1L, new BsonInt64(1L))))
                .Verifiable();

            // act
            target.Update(expected);

            // assert
            Mock.Verify(Mock.Get(context));
            Mock.Verify(Mock.Get(testCollection));
        }

        [Fact]
        public void GivenADocumentRepository_WhenRemoveIsCalled_CollectionIsTakenFromContextAndDeleteOneIsCalled()
        {
            // arrange
            var target = new TestDocumentRepository(context, logger);
            var testCollection = Mock.Of<IMongoCollection<TestDocument>>();
            var id = Guid.NewGuid();
            var expectedJsonFilter = "{ \"_id\" : CSUUID(\"" + id + "\") }";

            Mock.Get(context)
                .Setup(x => x.GetCollection<TestDocument>(typeof(TestDocument).Name))
                .Returns(testCollection)
                .Verifiable();

            Mock.Get(context)
                .Setup(x => x.AddCommand(It.IsAny<Func<Task>>()))
                .Callback(async (Func<Task> func) => { await func.Invoke().ConfigureAwait(false); })
                .Verifiable();

            Mock.Get(testCollection)
                .Setup(x => x.DeleteOneAsync(
                    It.Is<FilterDefinition<TestDocument>>(filter => filter.RenderToJson().Equals(expectedJsonFilter)),
                    default))
                .Returns(Task.FromResult((DeleteResult)new DeleteResult.Acknowledged(1L)))
                .Verifiable();

            // act
            target.Remove(id);

            // assert
            Mock.Verify(Mock.Get(context));
            Mock.Verify(Mock.Get(testCollection));
        }

        [Fact]
        public async Task GivenADocumentRepository_WhenGetAllIsCalled_CollectionIsTakenFromContext()
        {
            // arrange
            var target = new TestDocumentRepository(context, logger);
            var testCollection = Mock.Of<IMongoCollection<TestDocument>>();
            var findCollection = Mock.Of<IAsyncCursor<TestDocument>>();
            const string expectedJsonFilter = "{ }";
            int index = 0;
            var expected = new[] { new TestDocument(), new TestDocument() };

            Mock.Get(context)
                .Setup(x => x.GetCollection<TestDocument>(typeof(TestDocument).Name))
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
                    It.Is<FilterDefinition<TestDocument>>(filter => filter.RenderToJson().Equals(expectedJsonFilter)),
                    It.IsAny<FindOptions<TestDocument, TestDocument>>(),
                    default))
                .Returns(Task.FromResult(findCollection))
                .Verifiable();

            // act
            var actual = await target.GetAll().ConfigureAwait(false);

            // assert
            Assert.Equal(expected, actual);
            Mock.Verify(Mock.Get(context));
            Mock.Verify(Mock.Get(testCollection));
        }

        [Fact]
        public async Task GivenADocumentRepository_WhenGetByIdIsCalled_CollectionIsTakenFromContext()
        {
            // arrange
            var target = new TestDocumentRepository(context, logger);
            var testCollection = Mock.Of<IMongoCollection<TestDocument>>();
            var id = Guid.NewGuid();
            var findCollection = Mock.Of<IAsyncCursor<TestDocument>>();
            var expected = new TestDocument();
            var expectedJsonFilter = "{ \"_id\" : CSUUID(\"" + id + "\") }";
            Mock.Get(context)
                .Setup(x => x.GetCollection<TestDocument>(typeof(TestDocument).Name))
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
                    It.Is<FilterDefinition<TestDocument>>(filter => filter.RenderToJson().Equals(expectedJsonFilter)),
                    It.IsAny<FindOptions<TestDocument, TestDocument>>(),
                    default))
                .Returns(Task.FromResult(findCollection))
                .Verifiable();

            // act
            var actual = await target.GetById(id).ConfigureAwait(false);

            // assert
            Assert.Equal(expected, actual);
            Mock.Verify(Mock.Get(context));
            Mock.Verify(Mock.Get(testCollection));
        }

        [Fact]
        public async Task GivenAnErrorGettingTheCollection_WhenGetAllIsCalled_ExceptionIsThrown()
        {
            // arrange
            var target = new TestDocumentRepository(context, logger);
            Mock.Get(context)
                .Setup(x => x.GetCollection<TestDocument>(typeof(TestDocument).Name))
                .Throws(new Exception("Test error message"))
                .Verifiable();

            // act
            var actual = await Assert.ThrowsAsync<Exception>(async () =>
                    await target.GetAll().ConfigureAwait(false))
                .ConfigureAwait(false);

            // assert
            Assert.Equal("Test error message", actual.Message);
            Mock.Verify(Mock.Get(context));
        }

        public class TestDocumentRepository : DocumentRepositoryBase<TestDocument>
        {
            public TestDocumentRepository(IMongoContext context, ILogger logger)
                : base(context, logger)
            {
            }
        }

        public class TestDocument : IndexedEntityBase
        {
        }
    }
}
