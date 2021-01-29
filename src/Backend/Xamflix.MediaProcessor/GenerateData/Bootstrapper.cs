using Microsoft.Extensions.DependencyInjection;

namespace Xamflix.MediaProcessor.GenerateData
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddGenerateDataPipeline(this IServiceCollection services)
        {
            services.AddTransient<GenerateDataContext>();
            services.AddTransient<GenerateDataPipelineFactory>();
            services.AddTransient<RefreshRealmCommand>();
            services.AddTransient<LoadMovieImportsCommand>();
            services.AddTransient<GeneratePeopleCommand>();
            services.AddTransient<GenerateCategoriesCommand>();
            services.AddTransient<GenerateGenresCommand>();
            services.AddTransient<GenerateMoviesCommand>();
            services.AddTransient<UploadMovieImagesCommand>();
            services.AddTransient<UploadMovieTrailersCommand>();
            services.AddTransient<BuildBillboardCommand>();
            return services;
        }
    }
}