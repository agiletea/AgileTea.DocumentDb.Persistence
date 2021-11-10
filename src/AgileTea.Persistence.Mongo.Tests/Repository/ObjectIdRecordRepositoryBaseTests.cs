using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Common.Records;
using AgileTea.Persistence.Mongo.Context;
using AgileTea.Persistence.Mongo.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
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

        protected override TestObjectIdRecordRepository CreateRepository()
        {
            return new TestObjectIdRecordRepository(Context, LoggerFactory.CreateLogger<TestObjectIdRecordRepository>());
        }
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
    public class TestObjectIdRecord : IndexedRecordBase<ObjectId>
    {
    }
}
