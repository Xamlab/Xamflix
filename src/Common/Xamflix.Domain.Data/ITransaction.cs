using System;
using System.Threading.Tasks;

namespace Xamflix.Domain.Data
{
    public interface ITransaction : IDisposable
    {
        Task CommitAsync();
        void Rollback();
    }
}