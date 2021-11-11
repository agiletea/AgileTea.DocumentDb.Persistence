using System.Diagnostics.CodeAnalysis;
using AgileTea.Persistence.Common.Interfaces;

namespace AgileTea.Persistence.Common.Records
{
    [ExcludeFromCodeCoverage]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Not for records")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:Closing parenthesis should be spaced correctly", Justification = "Not for records")]
    public abstract record IndexedRecordBase<T>(T Id) : IIndexedEntity<T>;
}
