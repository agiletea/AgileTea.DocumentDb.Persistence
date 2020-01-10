using System;
using System.Threading.Tasks;

namespace AgileTea.Persistence.Common.Contexts
{
    public interface IDbContext : IDisposable
    {
        void AddCommand(Func<Task> func);
        Task<int> SaveChangesAsync();
    }
}
