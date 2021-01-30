using Microsoft.Extensions.DependencyInjection;
using Xamflix.App.Forms.Services;
using Xamflix.App.iOS.Services.Implementation;
using Xamflix.Core.Services;

namespace Xamflix.App.iOS
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddIos(this IServiceCollection services)
        {
            services.AddTransient<ISystemPathService, SystemPathService>();
            services.AddTransient<IViewCoordinatesService, ViewCoordinatesService>();
            return services;
        }
    }
}