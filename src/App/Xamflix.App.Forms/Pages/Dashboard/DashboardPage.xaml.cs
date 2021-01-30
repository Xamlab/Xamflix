using System;
using System.Threading.Tasks;
using MediaManager;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamflix.App.Forms.Pages.Movie;
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
        private MovieDetailsPopupView? _detailsPopupView = null;
        private BoxView? _background = null;
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
            ShowPopupBackground();
            ShowPopup(view);
        }

        private void ShowPopup(View view)
        {
            if (CrossMediaManager.Current.IsPlaying())
            {
                CrossMediaManager.Current.Stop();
            }
            var positionInParent = _viewCoordinatesService.GetCoordinates(view);
            var absoluteLayoutInParent = _viewCoordinatesService.GetCoordinates(SizerView);

            _detailsPopupView = new MovieDetailsPopupView((Domain.Models.Movie)view.BindingContext);

            var absoluteX = positionInParent.X - absoluteLayoutInParent.X;
            var absoluteY = positionInParent.Y - absoluteLayoutInParent.Y;
            var popupInitialFrame = new Rectangle((int)absoluteX, (int)absoluteY, (int)view.Width, (int)view.Height);
            RootLayout.Children.Add(_detailsPopupView, popupInitialFrame);
            _detailsPopupView.Open(this, popupInitialFrame, RootLayout.Width, RootLayout.Height);
        }

        private void ShowPopupBackground()
        {
            _background ??= new BoxView
            {
                BackgroundColor = Color.Black,
                Opacity = 0,
                GestureRecognizers = { new TapGestureRecognizer
                {
                    Command = new Command(HidePopup)
                }}
            };
            RootLayout.Children.Add(_background, new Rectangle(0,0,1,1), AbsoluteLayoutFlags.All);
            _background.Animate("BackgroundFadeIn", v => _background.Opacity = v, 0, 0.8);
        }

        private void HidePopup()
        {
            _background?.Animate("BackgroundFadeOut", 
                v => _background.Opacity = v, 0.8, 0, 
                finished:(x1, x2) =>
                {
                    RootLayout.Children.Remove(_background);
                    _background = null;
                });

            _detailsPopupView?.Close(this, () =>
            {
                RootLayout.Children.Remove(_detailsPopupView);
                _detailsPopupView = null;
            });
        }
    }
}