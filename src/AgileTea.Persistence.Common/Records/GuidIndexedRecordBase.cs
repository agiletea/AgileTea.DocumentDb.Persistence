using System;
using System.Diagnostics.CodeAnalysis;

namespace AgileTea.Persistence.Common.Records
{
    [ExcludeFromCodeCoverage]
    public abstract record GuidIndexedRecordBase : IndexedRecordBase<Guid>
    {
    }
}
