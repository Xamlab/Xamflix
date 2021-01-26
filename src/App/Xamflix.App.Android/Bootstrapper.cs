using Android.Content;
using Microsoft.Extensions.DependencyInjection;
using Xamflix.App.Droid.Services.Implementation;
using Xamflix.Core.Services;

namespace Xamflix.App.Droid
{
    public static class Bootstrapper
    {
        public static IServiceCollection RegisterAndroidDependencies(this IServiceCollection services)
        {
            services.AddTransient<ISystemPathService, SystemPathService>();
            services.AddSingleton<Context>(MainApplication.Instance);
            return services;
        }
    }
}