using System.Threading.Tasks;

namespace Xamflix.Domain.Data.Realm.Implementation
{
    public class RealmFactory : IRealmFactory
    {
        private readonly IRealmConfigurationFactory _realmConfigurationFactory;
        private Realms.Realm? _realm;

        public RealmFactory(IRealmConfigurationFactory realmConfigurationFactory)
        {
            _realmConfigurationFactory = realmConfigurationFactory;
        }

        public async Task<Realms.Realm> GetDefaultSyncedRealmAsync()
        {
            return _realm ??= await Realms.Realm.GetInstanceAsync(await _realmConfigurationFactory.GetDefaultSyncedConfigurationAsync());
        }

        public async Task<Realms.Realm> GetDefaultLocalRealmAsync()
        {
            return _realm ??= await Realms.Realm.GetInstanceAsync(await _realmConfigurationFactory.GetDefaultLocalConfigurationAsync());
        }
    }
}