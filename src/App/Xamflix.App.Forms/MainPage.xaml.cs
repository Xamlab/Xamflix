using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using MediaManager;
using MediaManager.Library;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Player;
using Xamarin.Forms;
using PositionChangedEventArgs = MediaManager.Playback.PositionChangedEventArgs;

namespace Xamflix.App.Forms
{
    public partial class MainPage
    {
        private bool _collapsed;

        public MainPage()
        {
            InitializeComponent();
            CrossMediaManager.Current.BufferedChanged += CurrentOnBufferedChanged;
            CrossMediaManager.Current.MediaItemChanged += CurrentOnMediaItemChanged;
            CrossMediaManager.Current.MediaItemFailed += CurrentOnMediaItemFailed;
            CrossMediaManager.Current.MediaItemFinished += CurrentOnMediaItemFinished;
            CrossMediaManager.Current.PositionChanged += CurrentOnPositionChanged;
            CrossMediaManager.Current.StateChanged += CurrentOnStateChanged;
        }

        private async void CurrentOnStateChanged(object sender, StateChangedEventArgs e)
        {
            switch(e.State)
            {
                case MediaPlayerState.Playing:
                {
                    await PosterImage.FadeTo(0);
                    await Animate();
                    break;
                }
                default:
                {
                    await PosterImage.FadeTo(1);
                    break;
                }
            }
        }

        private void CurrentOnPositionChanged(object sender, PositionChangedEventArgs e)
        {
            Trace.WriteLine($"CurrentOnPositionChanged {e.Position}");
        }

        private void CurrentOnMediaItemFinished(object sender, MediaItemEventArgs e)
        {
            Trace.WriteLine($"CurrentOnMediaItemFinished {JsonSerializer.Serialize(e.MediaItem)}");
        }

        private void CurrentOnMediaItemFailed(object sender, MediaItemFailedEventArgs e)
        {
            Trace.WriteLine(
                            $"CurrentOnMediaItemFailed  {e.Message} {e.Exeption} {JsonSerializer.Serialize(e.MediaItem)}");
        }

        private void CurrentOnMediaItemChanged(object sender, MediaItemEventArgs e)
        {
            Trace.WriteLine($"CurrentOnMediaItemChanged {JsonSerializer.Serialize(e.MediaItem)}");
        }

        private void CurrentOnBufferedChanged(object sender, BufferedChangedEventArgs e)
        {
            Trace.WriteLine($"CurrentOnMediaItemFailed  {e.Buffered}");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(2000);
            var media = new MediaItem(
                Uri.EscapeUriString(
                    @"https://xamflixdevgwcmedia-gewc1.streaming.media.azure.net/c2c86ac8-5dbc-479d-9bad-4bcac71901d7/Pride%20&%20Prejudice.ism/manifest(format=m3u8-aapl)"))
            {
                await ScaleOutTitleImageAsync();
            }

            await ScaleDownTitleImageAsync();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            //if (_colapsed)
            //{
            //    await ScaleOutTitleImageAsync();
            //}
            //Play();
            await Animate();
        }

        private void Play()
        {
            var media = new MediaItem(Uri.EscapeUriString(@"https://xamflixdevgwcmedia-gewc1.streaming.media.azure.net/9f4ee191-59ff-48bf-bf53-2bfbec396d8b/The Midnight Sky  Final Trailer .ism/manifest(format=m3u8-aapl)"))
                        {
                            MediaType = MediaType.Hls
                        };
            TrailerVideoView.Source = media;
            Trace.WriteLine($"Setting media item  {JsonSerializer.Serialize(media)}");
        }
    }
}