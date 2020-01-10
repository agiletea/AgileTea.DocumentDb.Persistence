using System;
using System.Diagnostics.CodeAnalysis;

namespace AgileTea.Persistence.Common.Entities
{
    [ExcludeFromCodeCoverage]
    public abstract class IndexedEntityBase
    {
        public Guid Id { get; set; }
    }
}
