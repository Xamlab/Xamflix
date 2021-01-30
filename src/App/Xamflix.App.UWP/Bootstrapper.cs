using Microsoft.Extensions.DependencyInjection;
using Xamflix.App.Forms.Services;
using Xamflix.App.UWP.Services.Implementation;
using Xamflix.Core.Services;

namespace Xamflix.App.UWP
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddUwp(this IServiceCollection services)
        {
            services.AddTransient<ISystemPathService, SystemPathService>();
            services.AddTransient<IViewCoordinatesService, ViewCoordinatesService>();
            return services;
        }
    }
}