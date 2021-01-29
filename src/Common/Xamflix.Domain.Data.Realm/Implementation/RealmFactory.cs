using System.Threading.Tasks;

namespace Xamflix.Domain.Data.Realm.Implementation
{
    public class RealmFactory : IRealmFactory
    {
        private readonly IRealmConfigurationFactory _realmConfigurationFactory;

        public RealmFactory(IRealmConfigurationFactory realmConfigurationFactory)
        {
            _realmConfigurationFactory = realmConfigurationFactory;
        }

        public async Task<Realms.Realm> GetDefaultSyncedRealmAsync()
        {
            return await Realms.Realm.GetInstanceAsync(await _realmConfigurationFactory.GetDefaultSyncedConfigurationAsync());
        }

        public async Task<Realms.Realm> GetDefaultLocalRealmAsync()
        {
            return await Realms.Realm.GetInstanceAsync(await _realmConfigurationFactory.GetDefaultLocalConfigurationAsync());
        }
    }
}