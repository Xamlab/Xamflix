using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xamflix.Domain.Data.Realm;
using Xamflix.MediaProcessor.Configuration;
using Xamflix.MediaProcessor.GenerateData;

namespace Xamflix.MediaProcessor
{
    public static class Program
    {
        private static IServiceProvider Services { get; set; }
        public static async Task Main()
        {
            var services = new ServiceCollection();
            Services = services.SetupConfigs()
                               .AddRealm()
                               .AddConsoleDependencies()
                               .BuildServiceProvider();

            var generateDataPipelineCommandFactory = Services.GetRequiredService<GenerateDataPipelineFactory>();
            var result = await generateDataPipelineCommandFactory
                               .CreateGenerateDataPipelineCommand()
                               .ExecuteAsync(Services.GetRequiredService<GenerateDataContext>());
            
            if(result.IsSuccessful)
            {
                Console.WriteLine("Successfully completed data generation");
            }
            else
            {
                Console.WriteLine("Data generation has failed");
                Console.WriteLine(result.FailureMessage);
                if(result.Exception is AggregateException aggregateException)
                {
                    foreach(Exception exception in aggregateException.InnerExceptions)
                    {
                        Console.WriteLine(exception.Message);
                    }
                }
                else
                {
                    Console.WriteLine(result.Exception);
                }
            }

            
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
        }
    }
}