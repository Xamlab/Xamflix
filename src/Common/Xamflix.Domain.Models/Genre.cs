using System.Linq;
using MongoDB.Bson;
using Realms;

namespace Xamflix.Domain.Models
{
    public class Genre : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; }
        [Required] [MapTo("_partitionKey")] public string PartitionKey { get; set; } = null!;
        [Required] public string Name { get; set; } = null!;
        [Backlink(nameof(Movie.Genres))] public IQueryable<Movie> Movies { get; } = null!;
    }
}