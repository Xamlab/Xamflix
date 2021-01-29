using System;
using System.Globalization;
using Xamarin.Forms;

namespace Xamflix.App.Forms.Resources.Converters
{
    public class InverseVisibilityConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !VisibilityConverter.IsVisible(value);
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}