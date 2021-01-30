using System;
using System.Globalization;
using Xamarin.Forms;

namespace Xamflix.App.Forms.Resources.Converters
{
    public class SecondsToFormattedDurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var durationInSeconds = value as int?;
            if (durationInSeconds == null) return "";

            var timespan = TimeSpan.FromSeconds(durationInSeconds.Value);
            return $"{timespan.Hours}h {timespan.Minutes}m";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}