using System;
using System.Drawing;
using Windows.UI.Xaml;
using Xamarin.Forms;
using Xamflix.App.Forms.Services;
using Point = Windows.Foundation.Point;

namespace Xamflix.App.UWP.Services.Implementation
{
    public class ViewCoordinatesService : IViewCoordinatesService
    {
        public PointF GetCoordinates(VisualElement view)
        {
            var renderer = Xamarin.Forms.Platform.UWP.Platform.GetRenderer(view);
            var nativeView = renderer.GetNativeElement();
            var elementVisualRelative = nativeView.TransformToVisual(Window.Current.Content);
            Point point = elementVisualRelative.TransformPoint(new Point(0, 0));
            return new PointF((int)Math.Round(point.X), (int)Math.Round(point.Y));
        }
    }
}