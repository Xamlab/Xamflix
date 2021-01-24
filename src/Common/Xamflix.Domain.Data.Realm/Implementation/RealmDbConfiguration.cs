namespace Xamflix.Domain.Data.Realm.Implementation
{
    public class RealmDbConfiguration
    {
        public string DbFileName { get; set; } = null!;
        public string LocalDbPath { get; set; } = null!;
        public ulong SchemaVersion { get; set; }
    }
}