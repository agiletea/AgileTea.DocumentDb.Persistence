using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AgileTea.Persistence.Common.Records;
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
    public class GuidRecordRepositoryBaseTests : IdRepositoryBaseTests<Guid, TestGuidRecordRepository, TestGuidRecord>
    {
        private readonly Guid id = Guid.NewGuid();
        protected override Guid Id => id;
        protected override string ExpectedJsonIdFilter => "{ \"_id\" : CSUUID(\"" + Id + "\") }";

        [Fact]
        public void GivenADocumentRepository_WhenUpdateIsCalled_CollectionIsTakenFromContextAndReplaceOneIsCalled()
        {
            // arrange
            var target = CreateRepository();
            var expected = new TestGuidRecord(Id);
            var testCollection = Mock.Of<IMongoCollection<TestGuidRecord>>();

            Mock.Get(Context)
                .Setup(x => x.GetCollection<TestGuidRecord>(target.CollectionName))
                .Returns(testCollection)
                .Verifiable();

            Mock.Get(Context)
                .Setup(x => x.AddCommand(It.IsAny<Func<Task>>()))
                .Callback(async (Func<Task> func) => { await func.Invoke().ConfigureAwait(false); })
                .Verifiable();

            Mock.Get(testCollection)
                .Setup(x => x.ReplaceOneAsync(
                    It.Is<FilterDefinition<TestGuidRecord>>(filter => filter.RenderToJson().Equals(ExpectedJsonIdFilter)),
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

        protected override TestGuidRecordRepository CreateRepository()
        {
            return new TestGuidRecordRepository(Context, LoggerFactory.CreateLogger<TestGuidRecordRepository>());
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Reduces Test file bloat")]
    public class TestGuidRecordRepository : GuidDocumentRecordRepositoryBase<TestGuidRecord>
    {
        public TestGuidRecordRepository(IMongoContext context, ILogger logger)
            : base(context, logger)
        {
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Reduces Test file bloat")]
    public record TestGuidRecord : IndexedRecordBase<Guid>
    {
        private readonly Guid id;

        public TestGuidRecord()
        {
        }

        public TestGuidRecord(Guid id)
        {
            this.id = id;
        }

        public override Guid Id => id;
    }
}
