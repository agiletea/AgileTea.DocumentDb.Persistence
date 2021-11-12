using System;
using System.Diagnostics.CodeAnalysis;

namespace AgileTea.Persistence.Common.Records
{
    [ExcludeFromCodeCoverage]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Not with Positional Records")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:Closing parenthesis should be spaced correctly", Justification = "Not with Positional Records")]
    public abstract record GuidIndexedRecordBase(Guid Id) : IndexedRecordBase<Guid>(Id)
    {
    }
}
