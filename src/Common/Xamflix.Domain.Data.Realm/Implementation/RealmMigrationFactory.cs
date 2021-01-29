using System.Collections.Generic;

namespace Xamflix.Domain.Data.Realm.Implementation
{
    public class RealmMigrationFactory : IRealmMigrationFactory
    {
        public IEnumerable<IMigration> GetMigrations()
        {
            return new IMigration[]
                   {
                       // Add your migrations in here 
                   };
        }
    }
}