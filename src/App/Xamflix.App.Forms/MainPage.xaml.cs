using MediaManager;
using MediaManager.Library;
using Xamarin.Forms;

namespace Xamflix.App.Forms
{
    public partial class MainPage : ContentPage
    {
        private readonly MediaItem _media;

        public MainPage()
        {
            InitializeComponent();
            _media = new MediaItem(
                "https://xamflixdevgwcmedia-gewc1.streaming.media.azure.net/9f4ee191-59ff-48bf-bf53-2bfbec396d8b/The Midnight Sky  Final Trailer .ism/manifest(format=m3u8-aapl)")
            {
                MediaType = MediaType.Hls
            };
            VideoView.Source = _media;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //await CrossMediaManager.Current.Play(_media);
        }
    }
}
