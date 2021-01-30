using System.Drawing;
using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamflix.App.Forms.Services;

namespace Xamflix.App.Droid.Services.Implementation
{
    public class ViewCoordinatesService : IViewCoordinatesService
    {
        private readonly Context _context;

        public ViewCoordinatesService(Context context)
        {
            _context = context;
        }

        public PointF GetCoordinates(VisualElement view)
        {
            var renderer = Platform.GetRenderer(view);
            var nativeView = renderer.View;
            var location = new int[2];
            var density = _context.Resources!.DisplayMetrics!.Density;
            nativeView.GetLocationOnScreen(location);
            return new PointF(location[0] / density, location[1] / density);
        }
    }
}