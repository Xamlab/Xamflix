using Realms;
using Xamflix.Core.Services;

namespace Xamflix.Domain.Data.Realm.Implementation
{
    internal class RealmConfigurationFactory : IRealmConfigurationFactory
    {
        private readonly RealmDbConfiguration _configuration;
        private readonly ISystemPathService _systemPathService;
        private readonly IRealmMigrationFactory _migrationFactory;

        public RealmConfigurationFactory(RealmDbConfiguration configuration,
                                         ISystemPathService systemPathService,
                                         IRealmMigrationFactory migrationFactory)
        {
            _configuration = configuration;
            _systemPathService = systemPathService;
            _migrationFactory = migrationFactory;
        }

        public RealmConfigurationBase GetDefaultConfiguration()
        {
            var localDbPath = _systemPathService.GetLocalPath(_configuration.DbFileName);
            return GetConfigurationWithPath(localDbPath);
        }

        public RealmConfigurationBase GetConfigurationWithPath(string realmDbPath)
        {
            return new RealmConfiguration(realmDbPath)
                   {
                       SchemaVersion = _configuration.SchemaVersion,
                       MigrationCallback = MigrationCallback,
                       ObjectClasses = _configuration.RealmTypes
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