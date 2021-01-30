using System;
using Xamarin.Forms.Xaml;

namespace Xamflix.App.Forms.Pages.Dashboard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsualMovieCell
    {
        public event EventHandler<EventArgs> MovieTapped;
        public UsualMovieCell()
        {
            InitializeComponent();
        }

        private void MovieTappedHandler(object sender, EventArgs e)
        {
            MovieTapped(sender, e);
        }
    }
}