using System;
using System.Linq;
using Realms;

namespace Xamflix.Domain.Data.Realm.Implementation
{
    internal class RealmConfigurationFactory
    {
        private readonly RealmDbConfiguration _configuration;
        private readonly RealmMigrationFactory _migrationFactory;
        private readonly Type[] _realmTypes;

        public RealmConfigurationFactory(RealmDbConfiguration configuration,
                                         RealmMigrationFactory migrationFactory)
        {
            _configuration = configuration;
            _migrationFactory = migrationFactory;
            _realmTypes = GetType().Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(RealmObject))).ToArray();
        }

        public RealmConfigurationBase GetDefaultConfiguration()
        {
            return GetConfigurationWithPath(_configuration.LocalDbPath);
        }

        public RealmConfigurationBase GetConfigurationWithPath(string realmDbPath)
        {
            return new RealmConfiguration(realmDbPath)
                   {
                       SchemaVersion = _configuration.SchemaVersion,
                       MigrationCallback = MigrationCallback,
                       ObjectClasses = _realmTypes
                   };
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