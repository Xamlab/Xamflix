using System.Threading.Tasks;

namespace Xamflix.Domain.Data.Realm
{
    public interface IRealmFactory
    {
        Task<Realms.Realm> GetDefaultRealmAsync();
    }
}