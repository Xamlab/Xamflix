using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace Xamflix.Domain.Models
{
    public class Category : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; }
        [Required] [MapTo("_partitionKey")] public string PartitionKey { get; set; } = null!;
        [Required] public string Name { get; set; } = null!;
        public IList<Movie> Movies { get; } = null!;
    }
}