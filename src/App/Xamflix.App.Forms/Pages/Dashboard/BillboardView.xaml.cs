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
using Xamflix.App.Forms.MarkupExtensions;
using Xamflix.Core.AsyncVoid;
using Xamflix.ViewModels.Dashboard;
using PositionChangedEventArgs = MediaManager.Playback.PositionChangedEventArgs;

namespace Xamflix.App.Forms.Pages.Dashboard
{
    public partial class BillboardView
    {
        private bool _collapsed;

        public BillboardView()
        {
            InitializeComponent();
            CrossMediaManager.Current.StateChanged += CurrentOnStateChanged;
        }

        public void Play()
        {
            if (BindingContext is IDashboardViewModel dashboardViewModel)
            {
                var billboardMovieStreamingUrl = dashboardViewModel.Dashboard?.BillboardMovie?.StreamingUrl;
                if (billboardMovieStreamingUrl != null)
                {
                    var media = new MediaItem(billboardMovieStreamingUrl)
                    {
                        MediaType = MediaType.Hls
                    };
                    TrailerVideoView.Source = media;
                    Trace.WriteLine($"Setting media item  {JsonSerializer.Serialize(media)}");
                }
            }
        }

        [AsyncVoidCheckExemption("Bridging UI lifecycle with async code")]
        private async void CurrentOnStateChanged(object sender, StateChangedEventArgs e)
        {
            switch (e.State)
            {
                case MediaPlayerState.Playing:
                {
                    await PosterImage.FadeTo(0);
                    await Animate();
                    break;
                }
                case MediaPlayerState.Stopped:
                {
                    await ScaleOutTitleImageAsync();
                    break;
                }
                default:
                {
                    await PosterImage.FadeTo(1);
                    break;
                }
            }
        }

        private async Task ScaleDownTitleImageAsync()
        {
            await Task.Delay(7000);
            var parentAnimation = new Animation();

            // Title Image
            var titleImageScaleDown = new Animation(v => TitleImage.Scale = v, 1, 0.7, Easing.CubicOut);
            var titleImageFadeOut = new Animation(v => TitleImage.Opacity = v, 1, 0.5, Easing.Linear);
            var titleImageMoveDown = new Animation(v => TitleImage.TranslationY = v, TitleImage.TranslationY, TitleLabel.Height, Easing.Linear);

            // Title Label
            var titleLabelAnimation1 = new Animation(v => TitleLabel.Opacity = v, 1, 0, Easing.Linear);
            var titleLabelAnimation2 = new Animation(v => TitleLabel.TranslationY = v, TitleLabel.TranslationY, 60, Easing.Linear);

            // Title Description
            var descriptionAnimation1 = new Animation(v => DescriptionLabel.Opacity = v, 1, 0, Easing.Linear);
            var descriptionAnimation2 = new Animation(v => DescriptionLabel.TranslationY = v,
                DescriptionLabel.TranslationY, 60, Easing.Linear);

            // Buttons
            var buttonAnimation1 = new Animation(v => PlayButton.Opacity = v, 1, 0.6, Easing.Linear);
            var buttonAnimation2 = new Animation(v => MoreButton.Opacity = v, 1, 0.6, Easing.Linear);

            parentAnimation.Add(0.1, 1, titleImageScaleDown);
            parentAnimation.Add(0, 1, titleImageFadeOut);
            //parentAnimation.Add(0, 1, titleImageMoveDown);

            parentAnimation.Add(0, 0.3, titleLabelAnimation1);
            parentAnimation.Add(0, 0.5, titleLabelAnimation2);

            parentAnimation.Add(0, 0.3, descriptionAnimation1);
            parentAnimation.Add(0, 0.5, descriptionAnimation2);

            parentAnimation.Add(0, 1, buttonAnimation1);
            parentAnimation.Add(0, 1, buttonAnimation2);

            parentAnimation.Commit(this, "ScaleDownAnimation", 16, 1000);
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
            var titleLabelAnimation2 = new Animation(v => TitleLabel.TranslationY = v, TitleLabel.TranslationY, -5,
                Easing.Linear);

            // Title Description
            var descriptionAnimation1 = new Animation(v => DescriptionLabel.Opacity = v, 0, 1, Easing.Linear);
            var descriptionAnimation2 = new Animation(v => DescriptionLabel.TranslationY = v,
                DescriptionLabel.TranslationY, -5, Easing.Linear);

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

            parentAnimation.Commit(this, "ScaleOutAnimation", 16, 1000);
            _collapsed = false;
        }

        private async Task Animate()
        {
            if (_collapsed)
            {
                await ScaleOutTitleImageAsync();
            }

            await ScaleDownTitleImageAsync();
        }

        [AsyncVoidCheckExemption("Bridging UI lifecycle with async code")]
        private async void RepeatButtonClicked(object sender, EventArgs e)
        {
            if (_collapsed) await ScaleOutTitleImageAsync();

            Play();
        }

        private void MuteButtonClicked(object sender, EventArgs e)
        {
            if (CrossMediaManager.Current.Volume.Muted)
            {
                CrossMediaManager.Current.Volume.Muted = false;
                MuteButton.ImageSource = "unMute".GetImageSource();
            }
            else
            {
                CrossMediaManager.Current.Volume.Muted = true;
                MuteButton.ImageSource = "mute".GetImageSource();
            }
        }
    }
}