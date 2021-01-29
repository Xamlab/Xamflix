using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Xamflix.MediaProcessor.Models;

namespace Xamflix.MediaProcessor.Services.Implementation
{
    public class CsvMovieImportService : IMovieImportService
    {
        public async Task<IEnumerable<MovieImport>> GetMovieImportsAsync(string importFilePath)
        {
            using var reader = new StreamReader(importFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var movieImports = await csv.GetRecordsAsync<MovieImport>().ToArrayAsync(); 
            return movieImports;
        }
    }
}