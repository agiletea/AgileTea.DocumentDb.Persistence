namespace AgileTea.Persistence.Common.Interfaces
{
    public interface IIndexedEntity<out TId>
    {
        TId Id { get; }
    }
}
