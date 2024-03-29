﻿using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using Xamflix.MediaProcessor.Configuration;
using Xamflix.MediaProcessor.Models;

namespace Xamflix.MediaProcessor.GenerateData
{
    public class GenerateDataContext
    {
        public GenerateDataContext(ImportConfiguration configuration)
        {
            ImportRootDir = configuration.ImportRootDir;
            PartitionKey = configuration.PartitionKey;
        }
        
        public string ImportRootDir { get; }
        public string PartitionKey { get; }
        public MovieImport[] MovieImports { get; set; } = null!;
        public Dictionary<string, ObjectId> People { get; } = new();
        public Dictionary<string, ObjectId> Categories { get; } = new();
        public Dictionary<string, ObjectId> Genres { get; } = new();

        public Dictionary<MovieImport, ObjectId> Movies { get; } = new();
        public Dictionary<string, string> MovieTrailers { get; } = new();
        public int UploadTrailerBatchSize { get; set; } = 10;
        public bool ForceGenerateImages { get; set; }
        public bool ForceGenerateTrailers { get; set; }
        public int MaxMovieTrailersToUpload { get; set; }
        
        public IEnumerable<string> GetNonEmptyListFromCommaSeparatedValues(string commaSeparateString)
        {
            return commaSeparateString.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                      .Select(p => p.Trim())
                                      .Where(p => !string.IsNullOrWhiteSpace(p));
        }
    }
}