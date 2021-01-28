using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using Xamflix.Core.Pipeline;
using Xamflix.Domain.Data.Realm;
using Xamflix.Domain.Models;
using Xamflix.MediaProcessor.Models;

namespace Xamflix.MediaProcessor.GenerateData
{
    public class GenerateMoviesCommand : IPipelineCommand<GenerateDataContext, GenerateDataResult>
    {
        private readonly IRealmFactory _realmFactory;

        public GenerateMoviesCommand(IRealmFactory realmFactory)
        {
            _realmFactory = realmFactory;
        }

        public IPipelineCommand<GenerateDataContext, GenerateDataResult> Next { get; set; }

        public async Task<GenerateDataResult> ExecuteAsync(GenerateDataContext context, CancellationToken token = default)
        {
            try
            {
                Console.WriteLine("Generating movies");
                using var realm = await _realmFactory.GetDefaultSyncedRealmAsync();
                foreach(MovieImport movieImport in context.MovieImports)
                {
                    Movie createdMovie = null;
                    await realm.WriteAsync(r =>
                    {
                        var movie = r.All<Movie>().FirstOrDefault(m => m.Name == movieImport.Name
                                                                       && m.Year == movieImport.Year
                                                                       && m.DurationInSeconds == movieImport.DurationInMinutes * 60);
                        if(movie == null)
                        {
                            movie = r.Add(new Movie
                                          {
                                              Id = ObjectId.GenerateNewId(),
                                              Name = movieImport.Name,
                                              Synopsis = movieImport.Synopsis,
                                              Year = movieImport.Year,
                                              DurationInSeconds = movieImport.DurationInMinutes * 60,
                                              MaturityRating = movieImport.MaturityRating,
                                              PartitionKey = context.PartitionKey
                                          });
                        }

                        foreach(var categoryName in context.GetNonEmptyListFromCommaSeparatedValues(movieImport.Categories))
                        {
                            Category category = realm.Find<Category>(context.Categories[categoryName]);
                            category.Movies.Add(movie);
                        }

                        foreach(var actorName in context.GetNonEmptyListFromCommaSeparatedValues(movieImport.Cast))
                        {
                            var actor = realm.Find<Person>(context.People[actorName]);
                            movie.Cast.Add(actor);
                        }

                        foreach(var genreName in context.GetNonEmptyListFromCommaSeparatedValues(movieImport.Genres))
                        {
                            Genre genre = realm.Find<Genre>(context.Genres[genreName]);
                            movie.Genres.Add(genre);
                        }

                        createdMovie = movie;
                    });
                    context.Movies[movieImport] = createdMovie.Id;
                }
                
                Console.WriteLine("Movies generated successfully");
            }
            catch(Exception ex)
            {
                return new GenerateDataResult("Something went wrong while generating movies.", ex);
            }

            return await Next.ExecuteAsync(context, token);
        }
    }
}