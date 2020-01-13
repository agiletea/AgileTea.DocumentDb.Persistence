using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgileTea.Persistence.Common.Contexts;
using AgileTea.Persistence.Common.Entities;
using AgileTea.Persistence.Common.Repository;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AgileTea.Persistence.Common.Tests.Repository
{
    public class RepositoryBaseTests
    {
        [Fact]
        public void GivenARepositoryClass_WhenInstantiated_ThenDbContextIsSet()
        {
            // arrange
            var context = new TestContext();
            var logger = Mock.Of<ILogger>();

            // act
            var target = new TestRepository(context, logger);

            // assert
            Assert.Equal(context, target.DbContext);
        }

        [Fact]
        public void GivenARepositoryClass_WhenDisposed_ThenDbContextIsDisposed()
        {
            // arrange
            var context = new TestContext();
            var logger = Mock.Of<ILogger>();
            var target = new TestRepository(context, logger);

            // act
            target.Dispose();

            // assert
            Assert.True(context.IsDisposed);
        }

        public class TestRepository : RepositoryBase<TestEntity, TestContext>
        {
            public TestRepository(TestContext context, ILogger logger)
                : base(context, logger)
            {
            }

            public override void Add(TestEntity document)
            {
                throw new NotImplementedException();
            }

            public override Task<TestEntity> GetById(Guid id)
            {
                throw new NotImplementedException();
            }

            public override Task<IEnumerable<TestEntity>> GetAll()
            {
                throw new NotImplementedException();
            }

            public override void Update(TestEntity document)
            {
                throw new NotImplementedException();
            }

            public override void Remove(Guid id)
            {
                throw new NotImplementedException();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell",
            "S3881:\"IDisposable\" should be implemented correctly",
            Justification = "Just a mock class for testing purposes")]
        public class TestContext : IDbContext
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }

            public void AddCommand(Func<Task> func)
            {
                throw new NotImplementedException();
            }

            public Task<int> SaveChangesAsync()
            {
                throw new NotImplementedException();
            }
        }

        public class TestEntity : IndexedEntityBase
        {
        }
    }
}
