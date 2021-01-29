using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms.Xaml;
using Xamflix.Core.AsyncVoid;
using Xamflix.ViewModels.Dashboard;

namespace Xamflix.App.Forms.Pages.Dashboard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage
    {
        private readonly IDashboardViewModel _viewModel; 
        public DashboardPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = App.Services.GetRequiredService<IDashboardViewModel>();
        }

        [AsyncVoidCheckExemption("Bridging UI lifecycle with async code")]
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadCommand.ExecuteAsync();
            await Task.Delay(1000);
            BillboardView.Play();
        }
    }
}