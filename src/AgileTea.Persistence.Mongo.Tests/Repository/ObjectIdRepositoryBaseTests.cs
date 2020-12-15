using AgileTea.Persistence.Common.Entities;
using AgileTea.Persistence.Mongo.Context;
using AgileTea.Persistence.Mongo.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Xunit;

namespace AgileTea.Persistence.Mongo.Tests.Repository
{
    public class ObjectIdRepositoryBaseTests : IdRepositoryBaseTests<ObjectId, TestObjectIdRepository, TestObjectIdDocument>
    {
        protected override ObjectId Id => new("507f1f77bcf86cd799439011");

        protected override TestObjectIdRepository CreateRepository()
        {
            return new TestObjectIdRepository(context, loggerFactory.CreateLogger<TestObjectIdRepository>());
        }

        protected override string ExpectedJsonIdFilter => "{ \"_id\" : ObjectId(\"" + Id + "\") }";

        [Fact]
        public void GivenInheritanceFromRespositoryBase_WhenCollectionNameIsSet_ThenItOverridesTheDefaultName()
        {
            // arrange
            var repository = CreateRepository();

            // assert
            Assert.Equal("TestCollectionName", repository.CollectionName);
        }
    }

    public class TestObjectIdRepository : ObjectIdDocumentRepositoryBase<TestObjectIdDocument>
    {
        public TestObjectIdRepository(IMongoContext context, ILogger logger)
            : base(context, logger)
        {
            CollectionName = "TestCollectionName";
        }
    }

    public class TestObjectIdDocument : IndexedEntityBase<ObjectId>
    { }
}
