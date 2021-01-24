using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamflix.App.Forms.MarkupExtensions
{
    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }

            var sourceAssembly = typeof(ImageResourceExtension).GetTypeInfo().Assembly;
            var resourcePath = $"Xamflix.App.Forms.Resources.Images.{Source}";
            var imageSource = ImageSource.FromResource(resourcePath, sourceAssembly);

            return imageSource;
        }
    }
}