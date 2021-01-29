using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using Xamflix.Core.Pipeline;
using Xamflix.Domain.Data.Realm;
using Xamflix.Domain.Models;

namespace Xamflix.MediaProcessor.GenerateData
{
    public class GenerateGenresCommand : IPipelineCommand<GenerateDataContext, GenerateDataResult>
    {
        private readonly IRealmFactory _realmFactory;

        public GenerateGenresCommand(IRealmFactory realmFactory)
        {
            _realmFactory = realmFactory;
        }

        public IPipelineCommand<GenerateDataContext, GenerateDataResult> Next { get; set; }

        public async Task<GenerateDataResult> ExecuteAsync(GenerateDataContext context, CancellationToken token = default)
        {
            try
            {
                Console.WriteLine("Generating genres");
                using var realm = await _realmFactory.GetDefaultSyncedRealmAsync();
                var genres = context.MovieImports.Select(m => context.GetNonEmptyListFromCommaSeparatedValues(m.Genres))
                                    .SelectMany(g => g)
                                    .ToHashSet();
                await realm.WriteAsync(r =>
                {
                    foreach(string genreName in genres)
                    {
                        var genre = r.All<Genre>().FirstOrDefault(g => g.Name == genreName);
                        if(genre == null)
                        {
                            genre = r.Add(new Genre
                                          {
                                              Id = ObjectId.GenerateNewId(),
                                              Name = genreName,
                                              PartitionKey = context.PartitionKey
                                          });
                        }

                        context.Genres[genreName] = genre.Id;
                    }
                });
                Console.WriteLine("Genres generated successfully");
            }
            catch(Exception ex)
            {
                return new GenerateDataResult($"Something went wrong while generating genres.", ex);
            }

            return await Next.ExecuteAsync(context, token);
        }
    }
}