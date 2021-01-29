using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Realms;
using Xamflix.Domain.Data.Realm.Implementation;
using Xamflix.Domain.Models;

namespace Xamflix.MediaProcessor.Configuration
{
    public static class Bootstrapper
    {
        public static IServiceCollection SetupConfigs(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", false, false)
                                .AddUserSecrets(typeof(Program).Assembly)
                                .AddEnvironmentVariables()
                                .Build();
            
            services.AddSingleton<IConfiguration>(configuration);

            var realmConfig = configuration.GetSection("Realm").Get<RealmDbConfiguration>();
            realmConfig.RealmTypes = typeof(Movie).Assembly
                                                  .GetTypes()
                                                  .Where(type => type.IsSubclassOf(typeof(RealmObject)))
                                                  .ToArray();
            services.AddSingleton(realmConfig);
            
            var mediaServiceConfiguration = configuration.GetSection("MediaService").Get<MediaServiceConfiguration>();
            services.AddSingleton(mediaServiceConfiguration);

            var blobConfiguration = configuration.GetSection("Blob").Get<BlobStorageConfiguration>();
            services.AddSingleton(blobConfiguration);
            
            var importConfiguration = configuration.GetSection("Import").Get<ImportConfiguration>();
            services.AddSingleton(importConfiguration);
            
            return services;
        }
    }
}