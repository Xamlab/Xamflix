using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamflix.Core.Pipeline;
using Xamflix.MediaProcessor.Models;
using Xamflix.MediaProcessor.Services;

namespace Xamflix.MediaProcessor.GenerateData
{
    public class LoadMovieImportsCommand : IPipelineCommand<GenerateDataContext, GenerateDataResult>
    {
        private readonly IMovieImportService _movieImportService;

        public LoadMovieImportsCommand(IMovieImportService movieImportService)
        {
            _movieImportService = movieImportService;
        }
        public IPipelineCommand<GenerateDataContext, GenerateDataResult> Next { get; set; } = null!;
        
        public async Task<GenerateDataResult> ExecuteAsync(GenerateDataContext context, CancellationToken token = default)
        {
            try
            {
                Console.WriteLine("Loading movie imports");
                var moviesFilePath = Path.Combine(context.ImportRootDir, "movies.csv");
                var movieImports = await _movieImportService.GetMovieImportsAsync(moviesFilePath);
                
                context.MovieImports = movieImports as MovieImport[] ?? movieImports.ToArray();
                
                if(context.MovieImports?.Any() != true)
                {
                    return new GenerateDataResult($"Could not locate any movies within {moviesFilePath}");
                }
                Console.WriteLine("Movie imports successfully loaded");
            }
            catch(Exception ex)
            {
                return new GenerateDataResult($"Something went wrong while fetching movie imports.", ex);
            }

            return await Next.ExecuteAsync(context, token);
        }
    }
}