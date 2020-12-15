using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AgileTea.Persistence.Common.Contexts;

namespace AgileTea.Persistence.Common.Repository
{
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name should match first type name",
        Justification = "File name allows for clarification that this is a generic class")]
    public abstract class RepositoryBase<TDocument, TContext, TId> : IRepository<TDocument, TId>
        where TContext : IDbContext
        where TDocument : class
    {
        private readonly TContext context;
        private string collectionName;

        protected RepositoryBase(TContext context)
        {
            this.context = context;
            collectionName = typeof(TDocument).Name;
        }

        public IDbContext DbContext => context;

        public virtual string CollectionName
        {
            get => collectionName;
            protected set => collectionName = value;
        }

        public abstract void Add(TDocument document);

        public abstract Task<TDocument> GetByIdAsync(TId id);

        public abstract TDocument GetById(TId id);

        public abstract Task<IEnumerable<TDocument>> GetAllAsync();

        public abstract IEnumerable<TDocument> GetAll();

        public abstract void Update(TDocument document);

        public abstract void Remove(TId id);

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
