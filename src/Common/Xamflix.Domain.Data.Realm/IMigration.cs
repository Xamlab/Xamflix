using Realms;

namespace Xamflix.Domain.Data.Realm
{
    public interface IMigration
    {
        void Migrate(Migration migration, ulong oldSchemaVersion);
    }
}