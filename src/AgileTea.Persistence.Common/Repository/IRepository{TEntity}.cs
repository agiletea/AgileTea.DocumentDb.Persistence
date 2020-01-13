using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgileTea.Persistence.Common.Repository
{
    public interface IRepository<TEntity> : IRepository, IDisposable
        where TEntity : class
    {
        void Add(TEntity document);
        Task<TEntity> GetById(Guid id);
        Task<IEnumerable<TEntity>> GetAll();
        void Update(TEntity document);
        void Remove(Guid id);
    }
}
