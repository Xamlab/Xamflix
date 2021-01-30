using System;
using System.Threading;
using System.Threading.Tasks;
using MediaManager;
using MediaManager.Library;
using MediaManager.Playback;
using MediaManager.Player;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamflix.App.Forms.MarkupExtensions;
using Xamflix.Core.AsyncVoid;

namespace Xamflix.App.Forms.Pages.Movie
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieDetailsPopupView
    {
        private readonly Domain.Models.Movie _movie;
        private bool _isPlaying;
        private CancellationTokenSource _delayCancellationTokenSource;
        public MovieDetailsPopupView(Domain.Models.Movie movie)
        {
            _movie = movie;
            InitializeComponent();
            BindingContext = movie;
            if (Device.RuntimePlatform == Device.UWP)
            {
                VideoControls.IsVisible = false;
            }
        }

        public void Open(IAnimatable owner, Rectangle source, double parentWidth, double parentHeight)
        {
            var parentAnimation = new Animation();
            _delayCancellationTokenSource = new CancellationTokenSource();
            // Title Image
            var deltaX = (parentWidth - source.Width) / 2 - source.X;
            var deltaY = (parentHeight - source.Height) / 2 - source.Y;
            var scaleX = new Animation(v => ScaleX = v, 1, 2.5, Easing.CubicIn);
            var scaleY = new Animation(v => ScaleY = v, 1, 3, Easing.CubicIn);
            var moveToCenterX = new Animation(v => TranslationX = v, 0, deltaX, Easing.CubicIn);
            var moveToCenterY = new Animation(v => TranslationY = v, 0, deltaY, Easing.CubicIn);

            var thumbnailFadeOut = new Animation(v => MovieThumbnailImage.Opacity = v, 1, 0, Easing.Linear);
            var posterImageFadeIn = new Animation(v => MoviePosterImage.Opacity = v, 0, 1, Easing.Linear);


            var posterTitleImageFadeIn = new Animation(v => MoviePosterTitleImage.Opacity = v, 0, 1, Easing.Linear);
            var contentFadeIn = new Animation(v => DetailsLayout.Opacity = v, 0, 1, Easing.Linear);

            parentAnimation.Add(0, 0.5, scaleX);
            parentAnimation.Add(0, 0.5, scaleY);
            parentAnimation.Add(0, 0.5, moveToCenterX);
            parentAnimation.Add(0, 0.5, moveToCenterY);
            parentAnimation.Add(0, 0.5, thumbnailFadeOut);
            parentAnimation.Add(0, 0.5, posterImageFadeIn);
            parentAnimation.Add(0.5, 1, posterTitleImageFadeIn);
            parentAnimation.Add(0.5, 1, contentFadeIn);

            parentAnimation.Commit(owner, "OpenAnimation", length: 500);

            if (Device.RuntimePlatform != Device.UWP)
            {
                Delay(Play, TimeSpan.FromSeconds(3));
            }
        }

        public async Task Close(IAnimatable owner, Action completion)
        {
            _delayCancellationTokenSource.Cancel();
            await CrossMediaManager.Current.Stop();
            var parentAnimation = new Animation();

            // Title Image
            var scaleX = new Animation(v => ScaleX = v, ScaleX, 1, Easing.CubicInOut);
            var scaleY = new Animation(v => ScaleY = v, ScaleY, 1, Easing.CubicOut);
            var moveBackX = new Animation(v => TranslationX = v, TranslationX, 0, Easing.CubicOut);
            var moveBackY = new Animation(v => TranslationY = v, TranslationY, 0, Easing.CubicOut);

            var thumbnailFadeIn = new Animation(v => MovieThumbnailImage.Opacity = v, 0, 1, Easing.Linear);
            var posterImageFadeOut = new Animation(v => MoviePosterImage.Opacity = v, 1, 0, Easing.Linear);


            var posterTitleImageFadeOut = new Animation(v => MoviePosterTitleImage.Opacity = v, 1, 0, Easing.Linear);
            var contentFadeOut = new Animation(v => DetailsLayout.Opacity = v, 1, 0, Easing.Linear);

            parentAnimation.Add(0, 0.5, posterTitleImageFadeOut);
            parentAnimation.Add(0, 0.5, contentFadeOut);
            parentAnimation.Add(0.5, 0.8, scaleX);
            parentAnimation.Add(0.5, 0.8, scaleY);
            parentAnimation.Add(0.5, 0.8, moveBackX);
            parentAnimation.Add(0.5, 0.8, moveBackY);
            parentAnimation.Add(0.5, 0.8, posterImageFadeOut);
            parentAnimation.Add(0.8, 1, thumbnailFadeIn);

            parentAnimation.Commit(owner, "CloseAnimation", length: 500, finished: (x1, x2) => completion());
        }

        private void CurrentOnStateChanged(object sender, StateChangedEventArgs e)
        {
            switch (e.State)
            {
                case MediaPlayerState.Playing:
                {
                    _isPlaying = true;
                    HidePosterVisuals();
                    break;
                }
                default:
                {
                    if (_isPlaying)
                    {
                        _isPlaying = false;
                        ShowPosterVisuals();
                    }

                    break;
                }
            }
        }

        private void HidePosterVisuals()
        {
            var parentAnimation = new Animation();

            var posterImageFadeOut = new Animation(v => MoviePosterImage.Opacity = v, 1, 0, Easing.Linear);
            var posterTitleImageFadeOut = new Animation(v => MoviePosterTitleImage.Opacity = v, 1, 0, Easing.Linear);
            var gradientOverlayFadeOut = new Animation(v => GradientOverlay.Opacity = v, 1, 0, Easing.Linear);


            parentAnimation.Add(0, 1, posterTitleImageFadeOut);
            parentAnimation.Add(0, 1, posterImageFadeOut);
            parentAnimation.Add(0, 1, gradientOverlayFadeOut);

            parentAnimation.Commit(this, "HidePosterVisuals");
        }

        private void ShowPosterVisuals()
        {
            var parentAnimation = new Animation();

            var posterImageFadeIn = new Animation(v => MoviePosterImage.Opacity = v, 0, 1, Easing.Linear);
            var posterTitleImageFadeIn = new Animation(v => MoviePosterTitleImage.Opacity = v, 0, 1, Easing.Linear);
            var gradientOverlayFadeIn = new Animation(v => GradientOverlay.Opacity = v, 0, 1, Easing.Linear);

            parentAnimation.Add(0, 1, posterImageFadeIn);
            parentAnimation.Add(0, 1, posterTitleImageFadeIn);
            parentAnimation.Add(0, 1, gradientOverlayFadeIn);

            parentAnimation.Commit(this, "ShowPosterVisuals");
        }

        public void Play()
        {
            if (_movie.StreamingUrl == null) return;

            CrossMediaManager.Current.StateChanged += CurrentOnStateChanged;
            var media = new MediaItem(_movie.StreamingUrl)
            {
                MediaType = MediaType.Hls
            };

            TrailerVideoView.Source = media;
        }

        private void RepeatButtonClicked(object sender, EventArgs e)
        {
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

        [AsyncVoidCheckExemption("Bridging UI lifecycle with async code")]
        private async void Delay(Action action, TimeSpan timeSpan)
        {
            try
            {
                await Task.Delay(timeSpan, _delayCancellationTokenSource.Token);
                action();
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}