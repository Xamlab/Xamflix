using System.Threading.Tasks;
using Realms;
using Realms.Sync;
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

        public async Task<RealmConfigurationBase> GetDefaultConfigurationAsync()
        {
            var app = App.Create(_configuration.CloudAppId);
            var user = await app.LogInAsync(Credentials.ApiKey(_configuration.ApiKey));
            var localDbPath = _systemPathService.GetLocalPath(_configuration.DbFileName);
            var configuration = new SyncConfiguration("DASHBOARD", user, localDbPath);
            return configuration;
        }

        private RealmConfigurationBase GetConfigurationWithPath(string realmDbPath)
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