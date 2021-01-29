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
        private bool _collapsed = false;

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
            await Task.Delay(1000);
            Play();
        }

        private async Task ScaleDownTitleImageAsync()
        {
            await Task.Delay(7000);
            var parentAnimation = new Animation();

            // Title Image
            var imageAnimation1 = new Animation(v => TitleImage.Scale = v, 1, 0.7, Easing.CubicOut);
            var imageAnimation2 = new Animation(v => TitleImage.Opacity = v, 1, 0.5, Easing.Linear);

            // Title Label
            var titleLabelAnimation1 = new Animation(v => TitleLabel.Opacity = v, 1, 0, Easing.Linear);
            var titleLabelAnimation2 = new Animation(v => TitleLabel.TranslationY = v, TitleLabel.TranslationY, 60, Easing.Linear);

            // Title Description
            var descriptionAnimation1 = new Animation(v => DescriptionLabel.Opacity = v, 1, 0, Easing.Linear);
            var descriptionAnimation2 = new Animation(v => DescriptionLabel.TranslationY = v, DescriptionLabel.TranslationY, 60, Easing.Linear);

            // Buttons
            var buttonAnimation1 = new Animation(v => PlayButton.Opacity = v, 1, 0.6, Easing.Linear);
            var buttonAnimation2 = new Animation(v => MoreButton.Opacity = v, 1, 0.6, Easing.Linear);

            parentAnimation.Add(0.1, 1, imageAnimation1);
            parentAnimation.Add(0, 1, imageAnimation2);

            parentAnimation.Add(0, 0.3, titleLabelAnimation1);
            parentAnimation.Add(0, 0.5, titleLabelAnimation2);

            parentAnimation.Add(0, 0.3, descriptionAnimation1);
            parentAnimation.Add(0, 0.5, descriptionAnimation2);

            parentAnimation.Add(0, 1, buttonAnimation1);
            parentAnimation.Add(0, 1, buttonAnimation2);

            parentAnimation.Commit(this, "ScaleDownAnimation", 16, 1000, null, null);
            _collapsed = true;
        }

        private async Task ScaleOutTitleImageAsync()
        {
            await Task.Delay(10);
            var parentAnimation = new Animation();

            // Title Image
            var imageAnimation1 = new Animation(v => TitleImage.Scale = v, 0.7, 1, Easing.CubicOut);
            var imageAnimation2 = new Animation(v => TitleImage.Opacity = v, 0.5, 1, Easing.Linear);

            // Title Label
            var titleLabelAnimation1 = new Animation(v => TitleLabel.Opacity = v, 0, 1, Easing.Linear);
            var titleLabelAnimation2 = new Animation(v => TitleLabel.TranslationY = v, TitleLabel.TranslationY, -5, Easing.Linear);

            // Title Description
            var descriptionAnimation1 = new Animation(v => DescriptionLabel.Opacity = v, 0, 1, Easing.Linear);
            var descriptionAnimation2 = new Animation(v => DescriptionLabel.TranslationY = v, DescriptionLabel.TranslationY, -5, Easing.Linear);

            // Buttons
            var buttonAnimation1 = new Animation(v => PlayButton.Opacity = v, 0.6, 1, Easing.Linear);
            var buttonAnimation2 = new Animation(v => MoreButton.Opacity = v, 0.6, 1, Easing.Linear);

            parentAnimation.Add(0.1, 1, imageAnimation1);
            parentAnimation.Add(0, 1, imageAnimation2);

            parentAnimation.Add(0, 0.3, titleLabelAnimation1);
            parentAnimation.Add(0, 0.5, titleLabelAnimation2);

            parentAnimation.Add(0, 0.3, descriptionAnimation1);
            parentAnimation.Add(0, 0.5, descriptionAnimation2);

            parentAnimation.Add(0, 1, buttonAnimation1);
            parentAnimation.Add(0, 1, buttonAnimation2);

            parentAnimation.Commit(this, "ScaleOutAnimation", 16, 1000, null, null);
            _collapsed = false;
        }

        private async Task Animate()
        {
            if(_collapsed)
            {
                await ScaleOutTitleImageAsync();
            }

            await ScaleDownTitleImageAsync();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if(_collapsed)
            {
                await ScaleOutTitleImageAsync();
            }

            Play();
        }

        private void Play()
        {
            var media = new MediaItem("https://xamflixdeveuams-usea.streaming.media.azure.net/ba1b04de-ad6d-46f0-8b5f-a1d36e49f005/Outside%20the%20wire.ism/manifest(format=m3u8-aapl)")
                        {
                            MediaType = MediaType.Hls
                        };
            TrailerVideoView.Source = media;
            Trace.WriteLine($"Setting media item  {JsonSerializer.Serialize(media)}");
        }
    }
}