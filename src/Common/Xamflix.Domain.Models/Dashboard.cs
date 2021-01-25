using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace Xamflix.Domain.Models
{
    public class Dashboard : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; }
        [Required] [MapTo("_partitionKey")] public string PartitionKey { get; set; } = null!;
        [Required] public string Name { get; set; } = null!;
        public Movie? BillboardMovie { get; set; }
        public IList<Category> Categories { get; } = null!;

    }
}