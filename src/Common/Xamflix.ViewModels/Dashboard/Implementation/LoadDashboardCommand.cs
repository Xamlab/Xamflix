using System.Threading;
using System.Threading.Tasks;
using Xamflix.Domain.Repositories;
using Xamflix.ViewModels.Base.Implementation;

namespace Xamflix.ViewModels.Dashboard.Implementation
{
    internal class LoadDashboardCommand : AsyncCommand
    {
        private readonly DashboardViewModel _viewModel;
        private readonly IDashboardRepository _dashboardRepository;

        public LoadDashboardCommand(DashboardViewModel viewModel,
                                    IDashboardRepository dashboardRepository)
        {
            _viewModel = viewModel;
            _dashboardRepository = dashboardRepository;
        }

        protected override async Task<bool> ExecuteCoreAsync(CancellationToken token = default)
        {
            _viewModel.Dashboard = await _dashboardRepository.GetDefaultDashboardAsync(token);
            return true;
        }
    }
}