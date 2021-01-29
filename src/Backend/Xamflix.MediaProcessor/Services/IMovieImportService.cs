using System.Collections.Generic;
using System.Threading.Tasks;
using Xamflix.MediaProcessor.Models;

namespace Xamflix.MediaProcessor.Services
{
    public interface IMovieImportService
    {
        Task<IEnumerable<MovieImport>> GetMovieImportsAsync(string csvImportFilePath);
    }
}