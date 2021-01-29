using PropertyChanged;
using Xamflix.Domain.Data;
using Xamflix.ViewModels.Base;
using Xamflix.ViewModels.Base.Implementation;

namespace Xamflix.ViewModels.Dashboard.Implementation
{
    [AddINotifyPropertyChangedInterface]
    internal class DashboardViewModel : BaseBindableObject, IDashboardViewModel
    {
        public DashboardViewModel(IAppDbContext dbContext)
        {
            LoadCommand = new LoadDashboardCommand(this, dbContext);
        }
        
        public IAsyncCommand LoadCommand { get; }
        public Domain.Models.Dashboard? Dashboard { get; internal set; }
    }
}