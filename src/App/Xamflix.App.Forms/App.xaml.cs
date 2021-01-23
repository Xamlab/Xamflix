using Xamarin.Forms;

namespace Xamflix.App.Forms
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var viewButton = new Button
            {
                Text = "View Video"
            };
            var contentPage = new ContentPage
            {
                Content = viewButton
            };
            viewButton.Command = new Command(() => contentPage.Navigation.PushAsync(new MainPage()));
            MainPage = new NavigationPage(contentPage);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
