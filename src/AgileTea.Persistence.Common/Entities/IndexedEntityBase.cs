using System.Diagnostics.CodeAnalysis;

namespace AgileTea.Persistence.Common.Entities
{
    [ExcludeFromCodeCoverage]
    public abstract class IndexedEntityBase<T>
    {
        public T Id { get; set; }
    }
}
