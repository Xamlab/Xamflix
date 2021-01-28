using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Xamflix.App.Forms.Effects;
using Xamflix.App.UWP;
using Grid = Xamarin.Forms.Grid;

[assembly: ResolutionGroupName("MyCompany")]
[assembly: ExportEffect(typeof(LabelShadowEffect), "LabelShadowEffect")]

namespace Xamflix.App.UWP
{
    public class LabelShadowEffect : PlatformEffect
    {
        private bool shadowAdded;

        protected override void OnAttached()
        {
            try
            {
                //TODO Does not work
                //if(!shadowAdded)
                //{
                //    var effect = (ShadowEffect) Element.Effects.FirstOrDefault(e => e is ShadowEffect);
                //    if(effect != null)
                //    {
                //        if(Control is TextBlock textBlock)
                //        {
                //            var shadowLabel = new Label
                //                              {
                //                                  Text = textBlock.Text,
                //                                  FontAttributes = FontAttributes.Bold,
                //                                  HorizontalOptions = LayoutOptions.Center,
                //                                  VerticalOptions = LayoutOptions.CenterAndExpand,
                //                                  TextColor = effect.Color,
                //                                  TranslationX = effect.DistanceX,
                //                                  TranslationY = effect.DistanceY
                //                              };

                //            var parentGrid = ((Grid) Element.Parent);
                //            if(parentGrid!=null)
                //            {
                //                var row = Grid.GetRow(Element);
                //                var column = Grid.GetColumn(Element);
                //                parentGrid.Children.Add(shadowLabel,row,column);
                //            } 
                //            ((Grid) Element.Parent).Children.Insert(0, shadowLabel);
                //        }
                //        shadowAdded = true;
                //    }
                //}
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
        }
    }
}