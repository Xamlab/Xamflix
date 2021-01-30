using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using MediaManager;
using MediaManager.Library;
using MediaManager.Playback;
using MediaManager.Player;
using Xamarin.Forms;
using Xamflix.App.Forms.MarkupExtensions;
using Xamflix.Core.AsyncVoid;
using Xamflix.ViewModels.Dashboard;

namespace Xamflix.App.Forms.Pages.Dashboard
{
    public partial class BillboardView
    {
        private bool _collapsed;
        private bool _isPlaying;

        public BillboardView()
        {
            InitializeComponent();
        }

        public void Play()
        {
            if (!(BindingContext is IDashboardViewModel dashboardViewModel)) return;

            var billboardMovieStreamingUrl = dashboardViewModel.Dashboard?.BillboardMovie?.StreamingUrl;
            if (billboardMovieStreamingUrl == null) return;

            CrossMediaManager.Current.StateChanged += CurrentOnStateChanged;
            var media = new MediaItem(billboardMovieStreamingUrl)
            {
                MediaType = MediaType.Hls
            };

            TrailerVideoView.Source = media;
            Trace.WriteLine($"Setting media item  {JsonSerializer.Serialize(media)}");
        }

        [AsyncVoidCheckExemption("Bridging UI lifecycle with async code")]
        private async void CurrentOnStateChanged(object sender, StateChangedEventArgs e)
        {
            switch (e.State)
            {
                case MediaPlayerState.Playing:
                {
                    _isPlaying = true;
                    await PosterImage.FadeTo(0);
                    await Animate();
                    break;
                }
                default:
                {
                    if (_isPlaying)
                    {
                        CrossMediaManager.Current.StateChanged -= CurrentOnStateChanged;
                        _isPlaying = false;
                        await ScaleOutTitleImageAsync();
                        await PosterImage.FadeTo(1);
                    }

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

            // Title Label
            var titleLabelFadeOut = new Animation(v => TitleLabel.Opacity = v, 1, 0, Easing.Linear);
            var titleLabelMoveDown = new Animation(v => TitleLabel.TranslationY = v, TitleLabel.TranslationY, 60,
                Easing.Linear);

            // Title Description
            var descriptionFadeOut = new Animation(v => DescriptionLabel.Opacity = v, 1, 0, Easing.Linear);
            var descriptionMoveDown = new Animation(v => DescriptionLabel.TranslationY = v,
                DescriptionLabel.TranslationY, 60, Easing.Linear);

            // Buttons
            var playButtonFadeOut = new Animation(v => PlayButton.Opacity = v, 1, 0.6, Easing.Linear);
            var moreButtonFadeOut = new Animation(v => MoreButton.Opacity = v, 1, 0.6, Easing.Linear);

            parentAnimation.Add(0.1, 1, titleImageScaleDown);
            parentAnimation.Add(0, 1, titleImageFadeOut);

            parentAnimation.Add(0, 0.3, titleLabelFadeOut);
            parentAnimation.Add(0, 0.5, titleLabelMoveDown);

            parentAnimation.Add(0, 0.3, descriptionFadeOut);
            parentAnimation.Add(0, 0.5, descriptionMoveDown);

            parentAnimation.Add(0, 1, playButtonFadeOut);
            parentAnimation.Add(0, 1, moreButtonFadeOut);

            parentAnimation.Commit(this, "ScaleDownAnimation", 16, 1000);
            _collapsed = true;
        }

        private async Task ScaleOutTitleImageAsync()
        {
            await Task.Delay(10);
            var parentAnimation = new Animation();

            // Title Image
            var titleImageScaleUp = new Animation(v => TitleImage.Scale = v, 0.7, 1, Easing.CubicOut);
            var titleImageFadeIn = new Animation(v => TitleImage.Opacity = v, 0.5, 1, Easing.Linear);

            // Title Label
            var titleLabelFadeIn = new Animation(v => TitleLabel.Opacity = v, 0, 1, Easing.Linear);
            var titleLabelMoveUp = new Animation(v => TitleLabel.TranslationY = v, TitleLabel.TranslationY, -5,
                Easing.Linear);

            // Title Description
            var descriptionFadeIn = new Animation(v => DescriptionLabel.Opacity = v, 0, 1, Easing.Linear);
            var descriptionMoveUp = new Animation(v => DescriptionLabel.TranslationY = v,
                DescriptionLabel.TranslationY, -5, Easing.Linear);

            // Buttons
            var playButtonFadeIn = new Animation(v => PlayButton.Opacity = v, 0.6, 1, Easing.Linear);
            var moreButtonFadeIn = new Animation(v => MoreButton.Opacity = v, 0.6, 1, Easing.Linear);

            parentAnimation.Add(0.1, 1, titleImageScaleUp);
            parentAnimation.Add(0, 1, titleImageFadeIn);

            parentAnimation.Add(0, 0.3, titleLabelFadeIn);
            parentAnimation.Add(0, 0.5, titleLabelMoveUp);

            parentAnimation.Add(0, 0.3, descriptionFadeIn);
            parentAnimation.Add(0, 0.5, descriptionMoveUp);

            parentAnimation.Add(0, 1, playButtonFadeIn);
            parentAnimation.Add(0, 1, moreButtonFadeIn);

            parentAnimation.Commit(this, "ScaleOutAnimation", 16, 1000);
            _collapsed = false;
        }

        private async Task Animate()
        {
            if (_collapsed) await ScaleOutTitleImageAsync();

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