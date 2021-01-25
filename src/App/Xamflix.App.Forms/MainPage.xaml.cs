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
    public partial class MainPage : ContentPage
    {
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
            switch (e.State)
            {
                case MediaPlayerState.Playing:
                    {
                        await PosterImage.FadeTo(0);
                        await ScaleDownTitleImageAsync();
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
            await Task.Delay(1000);
            var media = new MediaItem(
                Uri.EscapeUriString(
                    @"https://xamflixdevgwcmedia-gewc1.streaming.media.azure.net/9f4ee191-59ff-48bf-bf53-2bfbec396d8b/The Midnight Sky  Final Trailer .ism/manifest(format=m3u8-aapl)"))
            {
                MediaType = MediaType.Hls
            };
            TrailerVideoView.Source = media;
            Trace.WriteLine($"Setting media item  {JsonSerializer.Serialize(media)}");
        }

        private async Task ScaleDownTitleImageAsync()
        {
            await Task.Delay(10000);
            var parentAnimation = new Animation();
            

            // Title Image
            var imageAnimation1 = new Animation(v => TitleImage.WidthRequest = v, TitleImage.WidthRequest, 259, Easing.CubicOut);
            var imageAnimation2 = new Animation(v => TitleImage.HeightRequest = v, TitleImage.HeightRequest, 189, Easing.CubicOut);
            var imageAnimation3 = new Animation(v => TitleImage.Opacity = v, 1, 0.6, Easing.Linear);

            // Title Label
            var titleLabelAnimation1 = new Animation(v => TitleLabel.Opacity = v, 1, 0, Easing.Linear);
            var titleLabelAnimation2 = new Animation(v => TitleLabel.TranslationY = v, TitleLabel.TranslationY, -50, Easing.Linear);

            // Title Description
            var descriptionAnimation1 = new Animation(v => DescriptionLabel.Opacity = v, 1, 0, Easing.Linear);
            var descriptionAnimation2 = new Animation(v => DescriptionLabel.TranslationY = v, DescriptionLabel.TranslationY, -100, Easing.Linear);

            //Info Container
            var containerAnimation1 = new Animation(v => InfoContainer.TranslationY = v, InfoContainer.TranslationY, -130, Easing.Linear);

            // Buttons
            var buttonAnimation1 = new Animation(v => PlayButton.Opacity = v, 1, 0.6, Easing.Linear);
            var buttonAnimation2 = new Animation(v => MoreButton.Opacity = v, 1, 0.6, Easing.Linear);

            parentAnimation.Add(0, 1, imageAnimation1);
            parentAnimation.Add(0, 1, imageAnimation2);
            parentAnimation.Add(0, 1, imageAnimation3);

            parentAnimation.Add(0, 0.7, titleLabelAnimation1);
            parentAnimation.Add(0.2, 1, titleLabelAnimation2);

            parentAnimation.Add(0, 0.7, descriptionAnimation1);
            parentAnimation.Add(0.2, 1, descriptionAnimation2);

            parentAnimation.Add(0.2, 1, containerAnimation1);

            parentAnimation.Add(0, 1, buttonAnimation1);
            parentAnimation.Add(0, 1, buttonAnimation2);

            parentAnimation.Commit(this, "ScaleDownAnimation", 16, 1000, null, (v, c) => HideLables());

        }

        private void HideLables() 
        {
            //TitleLabel.IsVisible = false;
            //DescriptionLabel.IsVisible = false;
        }
    }
}