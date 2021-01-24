using Realms;

namespace Xamflix.Domain.Data.Realm
{
    public interface IRealmConfigurationFactory
    {
        RealmConfigurationBase GetDefaultConfiguration();
        RealmConfigurationBase GetConfigurationWithPath(string realmDbPath);
    }
}