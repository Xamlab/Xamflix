namespace Xamflix.MediaProcessor.Models
{
    public class MovieImport
    {
        public string Name { get; set; }
        public string Categories { get; set; }
        public string Synopsis { get; set; }
        public int Year { get; set; }
        public int DurationInMinutes { get; set; }
        public int MaturityRating { get; set; }
        public string Genres { get; set; }
        public string Cast { get; set; }
        public string TrailerFileName { get; set; }
        public string ThumbnailImageFileName { get; set; }
        public string PosterTitleImageFileName { get; set; }
        public string PosterImageFileName { get; set; }
        public string TallThumbnailFileName { get; set; }
    }
}