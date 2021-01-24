﻿using System.Linq;
using MongoDB.Bson;
using Realms;

namespace Xamflix.Domain.Models
{
    public class Person : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; }
        [Required] public string Name { get; set; } = null!;
        [Backlink(nameof(Movie.Cast))] public IQueryable<Movie> Movies { get; } = null!;
    }
}