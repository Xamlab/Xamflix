using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamflix.Core.Pipeline;
using Xamflix.Domain.Data.Realm;
using Xamflix.Domain.Models;
using Xamflix.MediaProcessor.Models;

namespace Xamflix.MediaProcessor.GenerateData
{
    public class UpdateMoviesWithTrailersCommand: IPipelineCommand<GenerateDataContext, GenerateDataResult>
    {
        private readonly IRealmFactory _realmFactory;
        private readonly Random _movieTrailerRandom = new Random();
        public UpdateMoviesWithTrailersCommand(IRealmFactory realmFactory)
        {
            _realmFactory = realmFactory;
        }

        public IPipelineCommand<GenerateDataContext, GenerateDataResult> Next { get; set; } = null!;

        public async Task<GenerateDataResult> ExecuteAsync(GenerateDataContext context, CancellationToken token = default)
        {
            Console.WriteLine("Updating movies with their trailers");
            using var realm = await _realmFactory.GetDefaultSyncedRealmAsync();
            foreach(MovieImport movieImport in context.MovieImports)
            {
                var movieId = context.Movies[movieImport];
                if(!context.MovieTrailers.TryGetValue($"trailer-{movieId}", out string? trailerUrl))
                {
                    var keys = context.MovieTrailers.Keys.ToArray();
                    var key = keys[_movieTrailerRandom.Next(0, keys.Length - 1)];
                    trailerUrl = context.MovieTrailers[key];
                }
                await realm.WriteAsync(r =>
                {
                    var movie = r.Find<Movie>(movieId);
                    movie.StreamingUrl = trailerUrl;
                });
            }
            Console.WriteLine("Movie trailers successfully updated");
            return await Next.ExecuteAsync(context, token);
        }
    }
}