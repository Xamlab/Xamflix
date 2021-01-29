using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamflix.Domain.Data;
using Xamflix.Domain.Models;

namespace Xamflix.Domain.Repositories.Implementation
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IAppDbContext _dbContext;

        public DashboardRepository(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Dashboard?> GetDefaultDashboardAsync(CancellationToken token = default)
        {
            await _dbContext.SynchronizeAsync(token);
            var dashboard = (await _dbContext.GetItemsAsync<Dashboard>())
                .FirstOrDefault(d => d.Name == "Dashboard");
            return dashboard;
        }
    }
}