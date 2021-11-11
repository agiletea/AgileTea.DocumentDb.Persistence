using System;
using System.Diagnostics.CodeAnalysis;

namespace AgileTea.Persistence.Common.Records
{
    [ExcludeFromCodeCoverage]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:Closing parenthesis should be spaced correctly", Justification = "Not for records")]
    public abstract record GuidIndexedRecordBase(Guid id) : IndexedRecordBase<Guid>(id)
    {
    }
}
