using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Common.Interfaces;

namespace AgileTea.Persistence.Common.Entities
{
    [ExcludeFromCodeCoverage]
    public abstract class IndexedEntityBase<T> : IIndexedEntity<T>
    {
        public virtual T Id { get; set; }
    }
}
