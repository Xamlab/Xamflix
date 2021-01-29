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
    public class GenerateCategoriesCommand : IPipelineCommand<GenerateDataContext, GenerateDataResult>
    {
        private readonly IRealmFactory _realmFactory;

        public GenerateCategoriesCommand(IRealmFactory realmFactory)
        {
            _realmFactory = realmFactory;
        }

        public IPipelineCommand<GenerateDataContext, GenerateDataResult> Next { get; set; } = null!;

        public async Task<GenerateDataResult> ExecuteAsync(GenerateDataContext context, CancellationToken token = default)
        {
            try
            {
                Console.WriteLine("Generating categories");
                var categories = context.MovieImports.Select(m => context.GetNonEmptyListFromCommaSeparatedValues(m.Categories))
                                        .SelectMany(c => c)
                                        .ToHashSet();
                using var realm = await _realmFactory.GetDefaultSyncedRealmAsync();
                await realm.WriteAsync(r =>
                {
                    foreach(string categoryName in categories)
                    {
                        var category = r.All<Category>().FirstOrDefault(c => c.Name == categoryName);
                        if(category == null)
                        {
                            category = r.Add(new Category
                                             {
                                                 Id = ObjectId.GenerateNewId(),
                                                 Name = categoryName,
                                                 PartitionKey = context.PartitionKey
                                             });
                        }

                        context.Categories[categoryName] = category.Id;
                    }
                });
                Console.WriteLine("Categories generated successfully");
            }
            catch(Exception ex)
            {
                return new GenerateDataResult("Something went wrong while generating categories.", ex);
            }

            return await Next.ExecuteAsync(context, token);
        }
    }
}