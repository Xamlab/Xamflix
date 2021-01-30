using System;
using System.Drawing;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamflix.App.Forms.Services;

namespace Xamflix.App.iOS.Services.Implementation
{
    public class ViewCoordinatesService : IViewCoordinatesService
    {
        public PointF GetCoordinates(VisualElement view)
        {
            var renderer = Platform.GetRenderer(view);
            var nativeView = renderer.NativeView;
            var rect = nativeView.Superview.ConvertPointToView(nativeView.Frame.Location, null);
            return new PointF((int) Math.Round(rect.X), (int) Math.Round(rect.Y));
        }
    }
}