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
                "https://devstreaming-cdn.apple.com/videos/streaming/examples/bipbop_16x9/bipbop_16x9_variant.m3u8")
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
