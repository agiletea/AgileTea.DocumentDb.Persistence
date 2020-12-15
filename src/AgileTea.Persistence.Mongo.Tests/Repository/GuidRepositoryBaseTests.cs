using System;
using AgileTea.Persistence.Common.Entities;
using AgileTea.Persistence.Mongo.Context;
using AgileTea.Persistence.Mongo.Repository;
using Microsoft.Extensions.Logging;

namespace AgileTea.Persistence.Mongo.Tests.Repository
{
    public class GuidRepositoryBaseTests : IdRepositoryBaseTests<Guid, TestGuidRepository, TestGuidDocument>
    {
        private readonly Guid id = Guid.NewGuid();

        protected override Guid Id => id;

        protected override TestGuidRepository CreateRepository()
        {
            return new TestGuidRepository(context, loggerFactory.CreateLogger<TestGuidRepository>());
        }

        protected override string ExpectedJsonIdFilter => "{ \"_id\" : CSUUID(\"" + Id + "\") }";
    }
    
    public class TestGuidRepository : GuidDocumentRepositoryBase<TestGuidDocument>
    {
        public TestGuidRepository(IMongoContext context, ILogger logger)
            : base(context, logger)
        {
        }
    }

    public class TestGuidDocument : IndexedEntityBase<Guid>
    { }
}
