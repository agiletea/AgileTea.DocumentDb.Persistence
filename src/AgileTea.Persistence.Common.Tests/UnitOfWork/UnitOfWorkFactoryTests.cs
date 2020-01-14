using AgileTea.Persistence.Common.Persistence;
using AgileTea.Persistence.Common.Repository;
using Moq;
using Xunit;

namespace AgileTea.Persistence.Common.Tests.UnitOfWork
{
    public class UnitOfWorkFactoryTests
    {
        [Fact]
        public void GivenARepository_WhenCreateUnitOfWorkIsCalled_ThenUnitOfWorkIsReturned()
        {
            // arrange
            var repository = Mock.Of<IRepository>();
            var target = new UnitOfWorkFactory();

            // act
            var actual = target.CreateUnitOfWork(repository);

            // assert
            Assert.IsType<Persistence.UnitOfWork>(actual);
        }
    }
}
