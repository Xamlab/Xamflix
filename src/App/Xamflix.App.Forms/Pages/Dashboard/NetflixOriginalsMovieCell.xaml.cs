using System;
using Xamarin.Forms.Xaml;

namespace Xamflix.App.Forms.Pages.Dashboard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NetflixOriginalsMovieCell
    {
        public event EventHandler<EventArgs> MovieTapped;
        public NetflixOriginalsMovieCell()
        {
            InitializeComponent();
        }

        private void MovieTappedHandler(object sender, EventArgs e)
        {
            MovieTapped(sender, e);
        }
    }
}