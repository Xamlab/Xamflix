using Android.Content;
using Microsoft.Extensions.DependencyInjection;
using Xamflix.App.Droid.Services.Implementation;
using Xamflix.App.Forms.Services;
using Xamflix.Core.Services;

namespace Xamflix.App.Droid
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddAndroid(this IServiceCollection services)
        {
            services.AddTransient<ISystemPathService, SystemPathService>();
            services.AddTransient<IViewCoordinatesService, ViewCoordinatesService>();
            services.AddSingleton<Context>(MainApplication.Instance);
            return services;
        }
    }
}