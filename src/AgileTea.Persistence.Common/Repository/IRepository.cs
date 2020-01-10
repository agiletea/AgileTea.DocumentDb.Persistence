using AgileTea.Persistence.Common.Contexts;

namespace AgileTea.Persistence.Common.Repository
{
    public interface IRepository
    {
        IDbContext DbContext { get; }
    }
}
