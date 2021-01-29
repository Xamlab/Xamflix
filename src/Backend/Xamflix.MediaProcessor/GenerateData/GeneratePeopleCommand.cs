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
    public class GeneratePeopleCommand : IPipelineCommand<GenerateDataContext, GenerateDataResult>
    {
        private readonly IRealmFactory _realmFactory;

        public GeneratePeopleCommand(IRealmFactory realmFactory)
        {
            _realmFactory = realmFactory;
        }

        public IPipelineCommand<GenerateDataContext, GenerateDataResult> Next { get; set; } = null!;

        public async Task<GenerateDataResult> ExecuteAsync(GenerateDataContext context, CancellationToken token = default)
        {
            try
            {
                Console.WriteLine("Generating people");
                var people = context.MovieImports.Select(m => context.GetNonEmptyListFromCommaSeparatedValues(m.Cast))
                                    .SelectMany(p => p)
                                    .ToHashSet();
                using var realm = await _realmFactory.GetDefaultSyncedRealmAsync();
                await realm.WriteAsync(r =>
                {
                    foreach(string personName in people)
                    {
                        var person = r.All<Person>().FirstOrDefault(p => p.Name == personName)
                                     ?? r.Add(new Person
                                              {
                                                  Id = ObjectId.GenerateNewId(),
                                                  Name = personName,
                                                  PartitionKey = context.PartitionKey
                                              });

                        context.People[personName] = person.Id;
                    }
                });
                Console.WriteLine("People generated successfully");
            }
            catch(Exception ex)
            {
                return new GenerateDataResult($"Something went wrong while generating people.", ex);
            }

            return await Next.ExecuteAsync(context, token);
        }
    }
}