using System.Threading.Tasks;
using AgileTea.Persistence.Common.Contexts;
using AgileTea.Persistence.Common.Repository;
using Moq;
using Xunit;

namespace AgileTea.Persistence.Common.Tests.UnitOfWork
{
    public class UnitOfWorkTests
    {
        [Fact]
        public void GivenAListOfCommands_WhenCommitAsyncIsNotCalled_ContextSaveChangesIsNotCalled()
        {
            // arrange
            var repository = Mock.Of<IRepository<TestEntity>>();
            var context = Mock.Of<IDbContext>();

            Mock.Get(repository).Setup(x => x.DbContext).Returns(context);

            // act
            using (new Persistence.UnitOfWork(repository))
            {
                var testEntity = new TestEntity
                {
                    TestProperty = "Value"
                };

                repository.Add(testEntity);
            }

            // verify
            Mock.Get(context).Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task GivenAListOfCommands_WhenCommitAsyncIsCalled_ContextSaveChangesIsCalled()
        {
            // arrange
            var repository = Mock.Of<IRepository<TestEntity>>();
            var context = Mock.Of<IDbContext>();

            Mock.Get(repository).Setup(x => x.DbContext).Returns(context);

            // act
            using (var target = new Persistence.UnitOfWork(repository))
            {
                var testEntity = new TestEntity
                {
                    TestProperty = "Value"
                };

                repository.Add(testEntity);
                await target.CommitAsync().ConfigureAwait(false);
            }

            // verify
            Mock.Get(context).Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        public class TestEntity
        {
            public string TestProperty { get; set; }
        }
    }
}
