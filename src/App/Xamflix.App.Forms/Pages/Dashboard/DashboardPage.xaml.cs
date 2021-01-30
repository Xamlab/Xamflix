using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;
using Xamflix.App.Forms.Services;
using Xamflix.Core.AsyncVoid;
using Xamflix.ViewModels.Dashboard;
using Rectangle = Xamarin.Forms.Rectangle;

namespace Xamflix.App.Forms.Pages.Dashboard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardPage
    {
        private readonly IDashboardViewModel _viewModel;
        private Xamarin.Forms.Shapes.Rectangle? _currentRectangle = null;
        private readonly IViewCoordinatesService _viewCoordinatesService;

        public DashboardPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = App.Services.GetRequiredService<IDashboardViewModel>();
            _viewCoordinatesService = App.Services.GetRequiredService<IViewCoordinatesService>();
        }

        [AsyncVoidCheckExemption("Bridging UI lifecycle with async code")]
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadCommand.ExecuteAsync();
            await Task.Delay(1000);
            BillboardView.Play();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (height >= 0)
            {
                BillboardView.HeightRequest = 0.8 * height;
            }
        }

        private void MovieTapped(object sender, EventArgs e)
        {
            var view = (View) sender;
            var positionInParent = _viewCoordinatesService.GetCoordinates(view);
            var absoluteLayoutInParent = _viewCoordinatesService.GetCoordinates(RootLayout);

            if (_currentRectangle != null)
            {
                RootLayout.Children.Remove(_currentRectangle);
            }
            _currentRectangle = new Xamarin.Forms.Shapes.Rectangle
            {
                Background = Brush.Red
            };
            var absoluteX = positionInParent.X - absoluteLayoutInParent.X;
            var absoluteY = positionInParent.Y - absoluteLayoutInParent.Y;
            RootLayout.Children.Add(_currentRectangle, new Rectangle(absoluteX, absoluteY, view.Width, view.Height));
        }
    }
}