using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgileTea.Persistence.Common.Repository
{

    public interface IRepository<TEntity> : IRepository, IDisposable
        where TEntity : class
    {
        void Add(TEntity obj);
        Task<TEntity> GetById(Guid id);
        Task<IEnumerable<TEntity>> GetAll();
        void Update(TEntity obj);
        void Remove(Guid id);
    }
}
