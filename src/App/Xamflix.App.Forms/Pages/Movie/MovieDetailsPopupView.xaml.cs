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

        private double _sourceX;
        private double _sourceY;
        private double _deltaX;
        private double _deltaY;

        private double _sourceWidth;
        private double _sourceHeight;
        private double _deltaWidth;
        private double _deltaHeight;

        public MovieDetailsPopupView(Domain.Models.Movie movie)
        {
            _movie = movie;
            InitializeComponent();
            BindingContext = movie;
        }

        public void Open(IAnimatable owner, Rectangle source, double parentWidth, double parentHeight)
        {
            var parentAnimation = new Animation();
            _delayCancellationTokenSource = new CancellationTokenSource();
            // Title Image
            var marginLarge = 100;
            var marginSmall = 50;
            var aspectRatio = 16.0 / 10.0;
            var maxHeight = 600;
            var maxWidth = maxHeight * aspectRatio;

            double width, height;
            if (parentWidth > parentHeight)
            {
                height = Math.Min(parentHeight - 2*marginSmall, maxHeight);
                width = Math.Min(height * aspectRatio, parentWidth - 2*marginLarge);
            }
            else
            {
                width = Math.Min(parentWidth - 2*marginSmall, maxWidth);
                height = Math.Min(width / aspectRatio, parentHeight - 2*marginLarge);
            }
            
            var x = (parentWidth - width) / 2;
            var y = (parentHeight - height) / 2;

            _sourceX = source.X;
            _sourceY = source.Y;
            _deltaX = x - _sourceX;
            _deltaY = y - _sourceY;

            _sourceWidth = source.Width;
            _sourceHeight = source.Height;
            _deltaWidth = width - _sourceWidth;
            _deltaHeight = height - _sourceHeight;

            var scale = new Animation(v => AbsoluteLayout.SetLayoutBounds(this, 
                new Rectangle(_sourceX + v*_deltaX, 
                    _sourceY + v*_deltaY, 
                    _sourceWidth + v*_deltaWidth, 
                    _sourceHeight + v*_deltaHeight)), 0, 1, Easing.CubicIn);
            

            var thumbnailFadeOut = new Animation(v => MovieThumbnailImage.Opacity = v, 1, 0, Easing.Linear);
            var posterImageFadeIn = new Animation(v => MoviePosterImage.Opacity = v, 0, 1, Easing.Linear);
            var videoControlsFadeIn = new Animation(v => VideoControls.Opacity = v, 0, 1, Easing.Linear);


            var posterTitleImageFadeIn = new Animation(v => MoviePosterTitleImage.Opacity = v, 0, 1, Easing.Linear);
            var contentFadeIn = new Animation(v => DetailsLayout.Opacity = v, 0, 1, Easing.Linear);

            parentAnimation.Add(0, 0.5, scale);
            parentAnimation.Add(0, 0.5, thumbnailFadeOut);
            parentAnimation.Add(0, 0.5, posterImageFadeIn);
            parentAnimation.Add(0.5, 1, posterTitleImageFadeIn);
            parentAnimation.Add(0.5, 1, contentFadeIn);
            parentAnimation.Add(0.7, 1, videoControlsFadeIn);

            parentAnimation.Commit(owner, "OpenAnimation", length: 500);

            Delay(Play, TimeSpan.FromSeconds(2));
        }

        public async Task Close(IAnimatable owner, Action completion)
        {
            _delayCancellationTokenSource.Cancel();
            if (CrossMediaManager.Current.IsPlaying())
            {
                await CrossMediaManager.Current.Stop();
            }
            var parentAnimation = new Animation();

            // Title Image
            var scale = new Animation(v => AbsoluteLayout.SetLayoutBounds(this,
                new Rectangle(_sourceX + v * _deltaX,
                    _sourceY + v * _deltaY,
                    _sourceWidth + v * _deltaWidth,
                    _sourceHeight + v * _deltaHeight)), 1, 0, Easing.CubicIn);

            var thumbnailFadeIn = new Animation(v => MovieThumbnailImage.Opacity = v, 0, 1, Easing.Linear);
            var posterImageFadeOut = new Animation(v => MoviePosterImage.Opacity = v, 1, 0, Easing.Linear);


            var posterTitleImageFadeOut = new Animation(v => MoviePosterTitleImage.Opacity = v, 1, 0, Easing.Linear);
            var contentFadeOut = new Animation(v => DetailsLayout.Opacity = v, 1, 0, Easing.Linear);
            var videoControlsFadeOut = new Animation(v => VideoControls.Opacity = v, 1, 0, Easing.Linear);

            parentAnimation.Add(0, 0.3, videoControlsFadeOut);
            parentAnimation.Add(0, 0.5, posterTitleImageFadeOut);
            parentAnimation.Add(0, 0.5, contentFadeOut);
            parentAnimation.Add(0.5, 0.8, scale);
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