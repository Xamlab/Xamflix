using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using Realms;
using Xamflix.Core.Pipeline;
using Xamflix.Domain.Data.Realm;
using Xamflix.Domain.Models;

namespace Xamflix.MediaProcessor.GenerateData
{
    public class BuildBillboardCommand : IPipelineCommand<GenerateDataContext, GenerateDataResult>
    {
        private readonly IRealmFactory _realmFactory;

        public BuildBillboardCommand(IRealmFactory realmFactory)
        {
            _realmFactory = realmFactory;
        }

        public IPipelineCommand<GenerateDataContext, GenerateDataResult> Next { get; set; } = null!;

        public async Task<GenerateDataResult> ExecuteAsync(GenerateDataContext context, CancellationToken token = default)
        {
            using var realm = await _realmFactory.GetDefaultSyncedRealmAsync();
            
            var dashboard = await GetDashboardAsync(realm, context);
            
            var billboardMovie = realm.All<Movie>().First(m => m.Name == "The Midnight Sky");
            
            await realm.WriteAsync(r =>
            {
                dashboard.BillboardMovie = billboardMovie;
                foreach(var category in r.All<Category>())
                {
                    if(!dashboard.Categories.Contains(category))
                    {
                        dashboard.Categories.Add(category);
                    }
                }
            });
            
            
            return await Next.ExecuteAsync(context, token);
        }

        private async Task<Dashboard> GetDashboardAsync(Realm realm, GenerateDataContext context)
        {
            var dashboard = realm.All<Dashboard>()
                                 .FirstOrDefault(d => d.Name == "Dashboard");
            if(dashboard == null)
            {
                await realm.WriteAsync(r =>
                {
                    dashboard = new Dashboard
                                {
                                    Id = ObjectId.GenerateNewId(),
                                    Name = "Dashboard",
                                    PartitionKey = context.PartitionKey
                                };
                    r.Add(dashboard);
                });
            }

            return dashboard;
        }
    }
}