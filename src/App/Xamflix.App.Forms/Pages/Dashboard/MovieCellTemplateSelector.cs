using Xamarin.Forms;
using Xamflix.Domain.Models;

namespace Xamflix.App.Forms.Pages.Dashboard
{
    public class MovieCellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UsualMovieDataTemplate { get; set; } = null!;
        public DataTemplate NetflixOriginalsDataTemplate { get; set; } = null!;

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is Category category && category.Name == "Netflix Originals") return NetflixOriginalsDataTemplate;

            return UsualMovieDataTemplate;
        }
    }
}