using Microsoft.Extensions.DependencyInjection;
using Xamflix.Domain.Repositories;
using Xamflix.Domain.Repositories.Implementation;

namespace Xamflix.Domain
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddTransient<IDashboardRepository, DashboardRepository>();
            return services;
        }
    }
}