using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace Xamflix.Domain.Models
{
    public class Movie : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; }

        [Required] public string Name { get; set; } = null!;

        [Required] public string Synopsis { get; set; } = null!;

        public string? PosterTitleImageUrl { get; set; }
        public string? PosterImageUrl { get; set; }

        public int Year { get; set; }
        public int DurationInSeconds { get; set; }
        public int MaturityRating { get; set; }

        public IList<Genre> Genres { get; } = null!;

        public IList<Person> Cast { get; } = null!;
    }
}