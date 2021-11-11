using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Common.Interfaces;

namespace AgileTea.Persistence.Common.Records
{
    [ExcludeFromCodeCoverage]
    public abstract record IndexedRecordBase<T> : IIndexedEntity<T>
    {
        public virtual T Id { get; }
    }
}
