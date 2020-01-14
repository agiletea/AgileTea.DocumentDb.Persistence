using System;
using System.Threading.Tasks;

namespace AgileTea.Persistence.Common.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> CommitAsync();
    }
}