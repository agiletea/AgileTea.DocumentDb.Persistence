using AgileTea.Persistence.Common.Repository;

namespace AgileTea.Persistence.Common.Persistence
{
    internal class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork CreateUnitOfWork(IRepository repository)
        {
            return new UnitOfWork(repository);
        }
    }
}
