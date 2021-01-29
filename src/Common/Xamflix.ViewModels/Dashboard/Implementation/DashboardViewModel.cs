using PropertyChanged;
using Xamflix.Domain.Repositories;
using Xamflix.ViewModels.Base;
using Xamflix.ViewModels.Base.Implementation;

namespace Xamflix.ViewModels.Dashboard.Implementation
{
    [AddINotifyPropertyChangedInterface]
    internal class DashboardViewModel : BaseBindableObject, IDashboardViewModel
    {
        public DashboardViewModel(IDashboardRepository dashboardRepository)
        {
            LoadCommand = new LoadDashboardCommand(this, dashboardRepository);
        }

        public IAsyncCommand LoadCommand { get; }
        public Domain.Models.Dashboard? Dashboard { get; internal set; }
    }
}