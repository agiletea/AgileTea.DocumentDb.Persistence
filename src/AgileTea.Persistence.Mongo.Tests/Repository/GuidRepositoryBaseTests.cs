using System;
using System.Diagnostics.CodeAnalysis;
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
    public class GuidRepositoryBaseTests : IdRepositoryBaseTests<Guid, TestGuidRepository, TestGuidDocument>
    {
        private readonly Guid id = Guid.NewGuid();
        protected override Guid Id => id;
        protected override string ExpectedJsonIdFilter => "{ \"_id\" : CSUUID(\"" + Id + "\") }";

        [Fact]
        public void GivenADocumentRepository_WhenUpdateIsCalled_CollectionIsTakenFromContextAndReplaceOneIsCalled()
        {
            // arrange
            var target = CreateRepository();
            var expected = new TestGuidDocument { Id = Id };
            var testCollection = Mock.Of<IMongoCollection<TestGuidDocument>>();

            Mock.Get(Context)
                .Setup(x => x.GetCollection<TestGuidDocument>(target.CollectionName))
                .Returns(testCollection)
                .Verifiable();

            Mock.Get(Context)
                .Setup(x => x.AddCommand(It.IsAny<Func<Task>>()))
                .Callback(async (Func<Task> func) => { await func.Invoke().ConfigureAwait(false); })
                .Verifiable();

            Mock.Get(testCollection)
                .Setup(x => x.ReplaceOneAsync(
                    It.Is<FilterDefinition<TestGuidDocument>>(filter => filter.RenderToJson().Equals(ExpectedJsonIdFilter)),
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

        protected override TestGuidRepository CreateRepository()
        {
            return new TestGuidRepository(Context, LoggerFactory.CreateLogger<TestGuidRepository>());
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Reduces Test file bloat")]
    public class TestGuidRepository : GuidDocumentRepositoryBase<TestGuidDocument>
    {
        public TestGuidRepository(IMongoContext context, ILogger logger)
            : base(context, logger)
        {
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Reduces Test file bloat")]
    public class TestGuidDocument : IndexedEntityBase<Guid>
    {
    }
}
