﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace Xamflix.App.Forms.Resources.Converters
{
    public class EscapeUrlsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString()?.Replace(" ", "%20");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}