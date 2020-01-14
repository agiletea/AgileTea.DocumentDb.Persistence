using AgileTea.Persistence.Common.Repository;

namespace AgileTea.Persistence.Common.Persistence
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateUnitOfWork(IRepository repository);
    }
}