using System;
using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Common.Records;
using AgileTea.Persistence.Mongo.Context;
using AgileTea.Persistence.Mongo.Repository;
using Microsoft.Extensions.Logging;

namespace AgileTea.Persistence.Mongo.Tests.Repository
{
    public class GuidRecordRepositoryBaseTests : IdRepositoryBaseTests<Guid, TestGuidRecordRepository, TestGuidRecord>
    {
        private readonly Guid id = Guid.NewGuid();
        protected override Guid Id => id;
        protected override string ExpectedJsonIdFilter => "{ \"_id\" : CSUUID(\"" + Id + "\") }";

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
    public class TestGuidRecord : IndexedRecordBase<Guid>
    {
    }
}
