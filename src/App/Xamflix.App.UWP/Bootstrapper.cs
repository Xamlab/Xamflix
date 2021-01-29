using Microsoft.Extensions.DependencyInjection;
using Xamflix.App.UWP.Services.Implementation;
using Xamflix.Core.Services;

namespace Xamflix.App.UWP
{
    public static class Bootstrapper
    {
        public static IServiceCollection RegisterUWPDependencies(this IServiceCollection services)
        {
            services.AddTransient<ISystemPathService, SystemPathService>();
            return services;
        }
    }
}