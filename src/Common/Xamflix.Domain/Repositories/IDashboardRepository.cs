using System.Threading;
using System.Threading.Tasks;
using Xamflix.Domain.Models;

namespace Xamflix.Domain.Repositories
{
    public interface IDashboardRepository
    {
        Task<Dashboard?> GetDefaultDashboardAsync(CancellationToken token = default);
    }
}