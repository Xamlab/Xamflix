using System;
using System.Linq;
using System.Threading.Tasks;
using Realms;

namespace Xamflix.Domain.Data.Realm.Implementation
{
    public class RealmFactory : IRealmFactory
    {
        private readonly RealmDbConfiguration _configuration;
        private readonly RealmMigrationFactory _migrationFactory;
        private readonly Type[] _realmTypes;
        private Realms.Realm? _realm;

        public RealmFactory(RealmDbConfiguration configuration,
                            RealmMigrationFactory migrationFactory)
        {
            _configuration = configuration;
            _migrationFactory = migrationFactory;
            _realmTypes = GetType().Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(RealmObject))).ToArray();
        }

        public async Task<Realms.Realm> GetDefaultRealmAsync()
        {
            return _realm ??= await Realms.Realm.GetInstanceAsync(new RealmConfiguration(_configuration.LocalDbPath)
                                                                  {
                                                                      SchemaVersion = _configuration.SchemaVersion,
                                                                      MigrationCallback = MigrationCallback,
                                                                      ObjectClasses = _realmTypes
                                                                  });
        }

        private void MigrationCallback(Migration migration, ulong oldSchemaVersion)
        {
            foreach(IMigration realmMigration in _migrationFactory.GetMigrations())
            {
                realmMigration.Migrate(migration, oldSchemaVersion);
            }
        }
    }
}