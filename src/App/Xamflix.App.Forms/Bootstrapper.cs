using Microsoft.Extensions.DependencyInjection;
using Xamflix.App.Forms.Configuration;
using Xamflix.Domain.Data.Realm;
using Xamflix.ViewModels;

namespace Xamflix.App.Forms
{
    public static class Bootstrapper
    {
        public static IServiceCollection CreateContainer()
        {
            return new ServiceCollection();
        }

        public static void BuildContainer(this IServiceCollection services)
        {
            App.Services = services.BuildServiceProvider();
        }

        public static IServiceCollection RegisterFormsDependencies(this IServiceCollection services)
        {
            return "local".SetupFormsConfigs()
                          .BuildAndRegister(services)
                          .AddRealm()
                          .AddViewModels();
        }
    }
}