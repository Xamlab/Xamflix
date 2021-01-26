using Microsoft.Extensions.DependencyInjection;
using Xamflix.Core.Services;
using Xamflix.MediaProcessor.Services;
using Xamflix.MediaProcessor.Services.Implementation;

namespace Xamflix.MediaProcessor
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddConsoleDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IMediaService, MediaService>();
            services.AddTransient<ISystemPathService, SystemPathService>();
            services.AddTransient<IMovieImportService, CsvMovieImportService>();
            return services;
        }
    }
}