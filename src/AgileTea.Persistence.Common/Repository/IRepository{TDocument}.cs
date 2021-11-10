using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace AgileTea.Persistence.Common.Repository
{
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name should match first type name",
        Justification = "Differentiating generic version of interface")]
    public interface IRepository<TDocument, TId> : IRepository, IDisposable
        where TDocument : new()
    {
        void Add(TDocument document);
        Task<TDocument> GetByIdAsync(TId id);
        TDocument GetById(TId id);
        Task<IEnumerable<TDocument>> GetAllAsync();
        IEnumerable<TDocument> GetAll();
        void Update(TDocument document);
        void Remove(TId id);
    }
}
