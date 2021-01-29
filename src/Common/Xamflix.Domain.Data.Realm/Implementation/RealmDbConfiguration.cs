using System;

namespace Xamflix.Domain.Data.Realm.Implementation
{
    public class RealmDbConfiguration
    {
        public string DbFileName { get; set; } = null!;
        public ulong SchemaVersion { get; set; }
        public string CloudAppId { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public Type[] RealmTypes { get; set; }
    }
}