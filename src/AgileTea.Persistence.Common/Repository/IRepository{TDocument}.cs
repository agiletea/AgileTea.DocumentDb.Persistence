using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgileTea.Persistence.Common.Repository
{
    public interface IRepository<TDocument> : IRepository, IDisposable
        where TDocument : class
    {
        void Add(TDocument document);
        Task<TDocument> GetByIdAsync(Guid id);
        TDocument GetById(Guid id);
        Task<IEnumerable<TDocument>> GetAllAsync();
        IEnumerable<TDocument> GetAll();
        void Update(TDocument document);
        void Remove(Guid id);
    }
}
