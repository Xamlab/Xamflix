using System.Drawing;
using Xamarin.Forms;

namespace Xamflix.App.Forms.Services
{
    public interface IViewCoordinatesService
    {
        PointF GetCoordinates(VisualElement view);
    }
}