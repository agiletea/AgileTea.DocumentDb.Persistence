﻿using System;
using System.Diagnostics.CodeAnalysis;
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
        protected override string ExpectedJsonIdFilter => "{ \"_id\" : CSUUID(\"" + Id + "\") }";

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
