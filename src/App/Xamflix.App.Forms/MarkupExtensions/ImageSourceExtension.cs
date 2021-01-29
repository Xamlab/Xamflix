using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamflix.App.Forms.MarkupExtensions
{
    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string? Source { get; set; }

        public object? ProvideValue(IServiceProvider serviceProvider)
        {
            return Source?.GetImageSource();
        }
    }

    public static class ImageResourceExtensions 
    {
        public static string? GetImageSource(this string source)
        {
            if (string.IsNullOrEmpty(source)) return null;

            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                {
                    string uwpResource = $"Assets/{source}.png";
                    return uwpResource;
                }
                case Device.iOS:
                {
                    return source;
                }
                default:
                {
                    string androidResource = $"ic_{source!.ToLower()}";
                    return androidResource;
                }
            }
        }
    }
}