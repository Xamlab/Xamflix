using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Realms;
using Xamflix.Domain.Data.Realm.Implementation;
using Xamflix.Domain.Models;

namespace Xamflix.App.Forms.Configuration
{
    public static class Bootstrapper
    {
        public static IConfigurationBuilder SetupFormsConfigs(this string environmentName)
        {
            var configs = new ConfigurationBuilder();
            var fileProvider = new EmbeddedFileProvider(typeof(Forms.Bootstrapper).Assembly, typeof(App).Namespace);
            configs.AddJsonFile(fileProvider, "appsettings.json", false, false)
                   .AddJsonFile(fileProvider, $"appsettings.{environmentName.ToLower()}.json", true, false);

            return configs;
        }
        
        public static IServiceCollection BuildAndRegister(this IConfigurationBuilder configurationBuilder, IServiceCollection services)
        {
            try
            {
                IConfigurationRoot configs = configurationBuilder.Build();
                services.AddSingleton<IConfiguration>(configs);

                var realmConfig = configs.GetSection("Realm").Get<RealmDbConfiguration>();
                realmConfig.RealmTypes = typeof(Movie).Assembly
                                                      .GetTypes()
                                                      .Where(type => type.IsSubclassOf(typeof(RealmObject)))
                                                      .ToArray();
                services.AddSingleton(realmConfig);
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
                throw;
            }
            return services;
        }
    }
}