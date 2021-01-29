using System;
using System.Linq;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamflix.App.Droid;
using Xamflix.App.Forms.Effects;
using Color = Android.Graphics.Color;

[assembly: ResolutionGroupName("MyCompany")]
[assembly: ExportEffect(typeof(LabelShadowEffect), "LabelShadowEffect")]

namespace Xamflix.App.Droid
{
    public class LabelShadowEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                var control = Control as TextView;
                var effect = (ShadowEffect) Element.Effects.FirstOrDefault(e => e is ShadowEffect);
                if(effect != null)
                {
                    float radius = effect.Radius;
                    float distanceX = effect.DistanceX;
                    float distanceY = effect.DistanceY;
                    Color color = effect.Color.ToAndroid();
                    control?.SetShadowLayer(radius, distanceX, distanceY, color);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
        }
    }
}