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

        public async Task<Realms.Realm> GetDefaultRealmAsync()
        {
            return _realm ??= await Realms.Realm.GetInstanceAsync(_realmConfigurationFactory.GetDefaultConfiguration());
        }
    }
}