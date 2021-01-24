using System.Collections.Generic;

namespace Xamflix.Domain.Data.Realm
{
    public interface IRealmMigrationFactory
    {
        IEnumerable<IMigration> GetMigrations();
    }
}