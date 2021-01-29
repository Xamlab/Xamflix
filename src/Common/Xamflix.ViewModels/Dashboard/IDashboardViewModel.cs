using System.ComponentModel;
using Xamflix.ViewModels.Base;

namespace Xamflix.ViewModels.Dashboard
{
    public interface IDashboardViewModel : INotifyPropertyChanged
    {
        IAsyncCommand LoadCommand { get; }
        Domain.Models.Dashboard? Dashboard { get; }
    }
}