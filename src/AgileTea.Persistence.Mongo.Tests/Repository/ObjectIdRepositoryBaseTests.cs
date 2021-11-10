﻿using System.Diagnostics.CodeAnalysis;
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

        protected override TestObjectIdRepository CreateRepository()
        {
            return new TestObjectIdRepository(Context, LoggerFactory.CreateLogger<TestObjectIdRepository>());
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Reduces Test file bloat")]
    public class TestObjectIdRepository : ObjectIdDocumentRepositoryBase<TestObjectIdDocument>
    {
        public TestObjectIdRepository(IMongoContext context, ILogger logger)
            : base(context, logger)
        {
            CollectionName = "TestCollectionName";
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Reduces Test file bloat")]
    public class TestObjectIdDocument : IndexedEntityBase<ObjectId>
    {
    }
}