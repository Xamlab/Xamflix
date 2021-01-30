using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamflix.App.Forms.Pages.Movie
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovieDetailsPopupView
    {
        private double _deltaX;
        private double _deltaY;

        public MovieDetailsPopupView(Domain.Models.Movie movie)
        {
            InitializeComponent();
            BindingContext = movie;
        }

        public void Open(IAnimatable owner, Rectangle source, double parentWidth, double parentHeight)
        {
            var parentAnimation = new Animation();

            // Title Image
            _deltaX = (parentWidth - source.Width) / 2 - source.X;
            _deltaY = (parentHeight - source.Height) / 2 - source.Y;
            var scaleX = new Animation(v => ScaleX = v, 1, 2.5, Easing.CubicIn);
            var scaleY = new Animation(v => ScaleY = v, 1, 3, Easing.CubicIn);
            var moveToCenterX = new Animation(v => TranslationX = v, 0, _deltaX, Easing.CubicIn);
            var moveToCenterY = new Animation(v => TranslationY = v, 0, _deltaY, Easing.CubicIn);

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
            
            parentAnimation.Commit(owner, "OpenAnimation", length:500);

        }

        public void Close(IAnimatable owner, Action completion)
        {
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

            parentAnimation.Commit(owner, "CloseAnimation", length: 500, finished:(x1, x2) => completion());
        }
    }
}