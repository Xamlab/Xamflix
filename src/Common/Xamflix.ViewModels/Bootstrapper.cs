using Microsoft.Extensions.DependencyInjection;
using Xamflix.ViewModels.Dashboard;
using Xamflix.ViewModels.Dashboard.Implementation;

namespace Xamflix.ViewModels
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddTransient<IDashboardViewModel, DashboardViewModel>();
            return services;
        }
    }
}