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
    public class ObjectIdRecordRepositoryBaseTests : IdRepositoryBaseTests<ObjectId, TestObjectIdRecordRepository, TestObjectIdRecord>
    {
        protected override ObjectId Id => new ("507f1f77bcf86cd799439011");

        protected override string ExpectedJsonIdFilter => "{ \"_id\" : ObjectId(\"" + Id + "\") }";

        [Fact]
        public void GivenInheritanceFromRepositoryBase_WhenCollectionNameIsSet_ThenItOverridesTheDefaultName()
        {
            // arrange
            var repository = CreateRepository();

            // assert
            Assert.Equal("TestCollectionName", repository.CollectionName);
        }

        [Fact]
        public void GivenADocumentRepository_WhenUpdateIsCalled_CollectionIsTakenFromContextAndReplaceOneIsCalled()
        {
            // arrange
            var target = CreateRepository();
            var expected = new TestObjectIdRecord(Id);
            var testCollection = Mock.Of<IMongoCollection<TestObjectIdRecord>>();

            Mock.Get(Context)
                .Setup(x => x.GetCollection<TestObjectIdRecord>(target.CollectionName))
                .Returns(testCollection)
                .Verifiable();

            Mock.Get(Context)
                .Setup(x => x.AddCommand(It.IsAny<Func<Task>>()))
                .Callback(async (Func<Task> func) => { await func.Invoke().ConfigureAwait(false); })
                .Verifiable();

            Mock.Get(testCollection)
                .Setup(x => x.ReplaceOneAsync(
                    It.Is<FilterDefinition<TestObjectIdRecord>>(filter => filter.RenderToJson().Equals(ExpectedJsonIdFilter)),
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

        protected override TestObjectIdRecordRepository CreateRepository()
        {
            return new TestObjectIdRecordRepository(Context, LoggerFactory.CreateLogger<TestObjectIdRecordRepository>());
        }

        protected override TestObjectIdRecord CreateDocument() => new (Id);
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Reduces Test file bloat")]
    public class TestObjectIdRecordRepository : ObjectIdDocumentRecordRepositoryBase<TestObjectIdRecord>
    {
        public TestObjectIdRecordRepository(IMongoContext context, ILogger logger)
            : base(context, logger)
        {
            CollectionName = "TestCollectionName";
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Reduces Test file bloat")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:Closing parenthesis should be spaced correctly", Justification = "Not for record initiation")]
    public record TestObjectIdRecord(ObjectId id) : IndexedRecordBase<ObjectId>(id);
}
