using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgileTea.Persistence.Common.Contexts;
using Microsoft.Extensions.Logging;

namespace AgileTea.Persistence.Common.Repository
{
    public abstract class RepositoryBase<TEntity, TContext> : IRepository<TEntity>
        where TContext : IDbContext
        where TEntity : class
    {
        private readonly ILogger logger;
        private readonly TContext context;

        protected RepositoryBase(
            TContext context,
            ILogger logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public IDbContext DbContext => context;

        public abstract void Add(TEntity document);

        public abstract Task<TEntity> GetById(Guid id);

        public abstract Task<IEnumerable<TEntity>> GetAll();

        public abstract void Update(TEntity document);

        public abstract void Remove(Guid id);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                context?.Dispose();
            }
        }
    }
}
