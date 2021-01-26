using Microsoft.Extensions.DependencyInjection;
using Xamflix.Domain.Data.Realm.Implementation;

namespace Xamflix.Domain.Data.Realm
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddRealm(this IServiceCollection services)
        {
            services.AddTransient<IRealmConfigurationFactory, RealmConfigurationFactory>();
            services.AddTransient<IRealmMigrationFactory, RealmMigrationFactory>();
            services.AddTransient<IDomainServiceLocator, DomainServiceLocator>();
            services.AddTransient<IAppDbContext, RealmDbContext>();
            services.AddSingleton<IRealmFactory, RealmFactory>();
            return services;
        }
    }
}