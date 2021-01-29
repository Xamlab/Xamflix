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
        private RealmConfigurationBase? _syncedConfiguration;
        public RealmConfigurationFactory(RealmDbConfiguration configuration,
                                         ISystemPathService systemPathService,
                                         IRealmMigrationFactory migrationFactory)
        {
            _configuration = configuration;
            _systemPathService = systemPathService;
            _migrationFactory = migrationFactory;
        }

        public async Task<RealmConfigurationBase> GetDefaultSyncedConfigurationAsync()
        {
            if(_syncedConfiguration != null)
            {
                return _syncedConfiguration;
            }
            
            var app = App.Create(_configuration.CloudAppId);
            var user = await app.LogInAsync(Credentials.ApiKey(_configuration.ApiKey));
            var localDbPath = _systemPathService.GetLocalPath(_configuration.DbFileName);
            _syncedConfiguration = new SyncConfiguration("DASHBOARD", user, localDbPath);
            return _syncedConfiguration;
        }

        public Task<RealmConfigurationBase> GetDefaultLocalConfigurationAsync()
        {
            var localDbPath = _systemPathService.GetLocalPath(_configuration.DbFileName);
            return Task.FromResult(GetConfigurationWithPath(localDbPath));
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