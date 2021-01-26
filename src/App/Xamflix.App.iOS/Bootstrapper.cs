using Microsoft.Extensions.DependencyInjection;
using Xamflix.App.iOS.Services.Implementation;
using Xamflix.Core.Services;

namespace Xamflix.App.iOS
{
    public static class Bootstrapper
    {
        public static IServiceCollection RegisterIosDependencies(this IServiceCollection services)
        {
            services.AddTransient<ISystemPathService, SystemPathService>();
            return services;
        }
    }
}