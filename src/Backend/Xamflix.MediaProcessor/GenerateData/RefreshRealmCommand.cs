using System;
using System.Threading;
using System.Threading.Tasks;
using Xamflix.Core.Pipeline;
using Xamflix.Domain.Data.Realm;

namespace Xamflix.MediaProcessor.GenerateData
{
    public class RefreshRealmCommand : IPipelineCommand<GenerateDataContext, GenerateDataResult>
    {
        private readonly IRealmFactory _realmFactory;

        public RefreshRealmCommand(IRealmFactory realmFactory)
        {
            _realmFactory = realmFactory;
        }

        public IPipelineCommand<GenerateDataContext, GenerateDataResult> Next { get; set; } = null!;
        public bool IsTerminal { get; set; }

        public async Task<GenerateDataResult> ExecuteAsync(GenerateDataContext context, CancellationToken token = default)
        {
            try
            {
                Console.WriteLine("Refreshing realm");
                
                using var realm = await _realmFactory.GetDefaultSyncedRealmAsync();
                await realm.RefreshAsync();
                
                Console.WriteLine("Realm successfully refreshed");
            }
            catch(Exception ex)
            {
                return new GenerateDataResult($"Something went wrong while refreshing realm.", ex);
            }

            return IsTerminal ? new GenerateDataResult(true) : await Next.ExecuteAsync(context, token);
        }
    }
}