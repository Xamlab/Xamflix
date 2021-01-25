using System.Threading.Tasks;
using Realms;

namespace Xamflix.Domain.Data.Realm
{
    public interface IRealmConfigurationFactory
    {
        Task<RealmConfigurationBase> GetDefaultConfigurationAsync();
    }
}