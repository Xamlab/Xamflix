using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamflix.Domain.Data;
using Xamflix.ViewModels.Base.Implementation;

namespace Xamflix.ViewModels.Dashboard.Implementation
{
    internal class LoadDashboardCommand : AsyncCommand
    {
        private readonly DashboardViewModel _viewModel;
        private readonly IAppDbContext _dbContext;

        public LoadDashboardCommand(DashboardViewModel viewModel, IAppDbContext dbContext)
        {
            _viewModel = viewModel;
            _dbContext = dbContext;
        }

        protected override async Task<bool> ExecuteCoreAsync(CancellationToken token = default)
        {
            await _dbContext.SynchronizeAsync(token);
            _viewModel.Dashboard = (await _dbContext.GetItemsAsync<Domain.Models.Dashboard>())
                .FirstOrDefault(d => d.Name == "Dashboard");
            return true;
        }
    }
}