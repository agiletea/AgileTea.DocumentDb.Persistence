using System.Threading.Tasks;
using AgileTea.Persistence.Common.Repository;

namespace AgileTea.Persistence.Common.Persistence
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly IRepository repository;

        public UnitOfWork(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task<bool> CommitAsync()
        {
            return await repository.DbContext.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                repository.DbContext?.Dispose();
            }
        }
    }
}
