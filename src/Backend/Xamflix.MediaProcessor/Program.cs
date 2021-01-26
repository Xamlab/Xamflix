using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Realms;
using Xamflix.Domain.Data.Realm;
using Xamflix.Domain.Models;
using Xamflix.MediaProcessor.Configuration;
using Xamflix.MediaProcessor.Models;
using Xamflix.MediaProcessor.Services;

namespace Xamflix.MediaProcessor
{
    public static class Program
    {
        private static IServiceProvider Services { get; set; }
        private const string ImportRootDir = @"C:\Projects\Xamlab\Xamflix\Movies";
        private const string DashboardPartitionKey = "DASHBOARD";

        private static Dictionary<string, Person> People { get; } = new();
        private static Dictionary<string, Category> Categories { get; } = new();
        private static Dictionary<string, Genre> Genres { get; } = new();

        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            Services = services.SetupConfigs()
                               .AddRealm()
                               .AddConsoleDependencies()
                               .BuildServiceProvider();
            var moviesFilePath = Path.Combine(ImportRootDir, "movies.csv");
            var movieImports = await Services.GetRequiredService<IMovieImportService>().GetMovieImportsAsync(moviesFilePath);
            var realm = await Services.GetRequiredService<IRealmFactory>().GetDefaultLocalRealmAsync();
            IEnumerable<MovieImport> imports = movieImports as MovieImport[] ?? movieImports.ToArray();
            await AddAllPeopleAsync(imports, realm);
            await AddAllCategories(imports, realm);
            await AddAllGenres(imports, realm);
            await AddAllMovies(imports, realm);
            
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
        }

        private static async Task AddAllMovies(IEnumerable<MovieImport> movieImports, Realm realm)
        {
            await realm.WriteAsync(r =>
            {
                foreach(MovieImport movieImport in movieImports)
                {
                    var movie = r.All<Movie>().FirstOrDefault(m => m.Name == movieImport.Name
                                                                   && m.Year == movieImport.Year
                                                                   && m.DurationInSeconds == movieImport.DurationInMinutes * 60);
                    if(movie == null)
                    {
                        movie = r.Add(new Movie
                                      {
                                          Id = ObjectId.GenerateNewId(),
                                          Name = movieImport.Name,
                                          Synopsis = movieImport.Synopsis,
                                          Year = movieImport.Year,
                                          DurationInSeconds = movieImport.DurationInMinutes * 60,
                                          MaturityRating = movieImport.MaturityRating,
                                          PartitionKey = DashboardPartitionKey
                                      });
                    }

                    foreach(var categoryName in movieImport.Categories.GetNonEmptyListFromCommaSeparatedValues())
                    {
                        Categories[categoryName].Movies.Add(movie);
                    }

                    foreach(var actorName in movieImport.Cast.GetNonEmptyListFromCommaSeparatedValues())
                    {
                        movie.Cast.Add(People[actorName]);
                    }
                    
                    foreach(var genreName in movieImport.Genres.GetNonEmptyListFromCommaSeparatedValues())
                    {
                        movie.Genres.Add(Genres[genreName]);
                    }
                }
            });
        }

        private static async Task AddAllPeopleAsync(IEnumerable<MovieImport> movieImports, Realm realm)
        {
            var people = movieImports.Select(m => m.Cast.GetNonEmptyListFromCommaSeparatedValues())
                                     .SelectMany(p => p)
                                     .ToHashSet();
            await realm.WriteAsync(r =>
            {
                foreach(string personName in people)
                {
                    var person = r.All<Person>().FirstOrDefault(p => p.Name == personName);
                    if(person == null)
                    {
                        person = r.Add(new Person
                                       {
                                           Id = ObjectId.GenerateNewId(),
                                           Name = personName,
                                           PartitionKey = DashboardPartitionKey
                                       });
                    }

                    People[personName] = person;
                }
            });
        }

        private static async Task AddAllCategories(IEnumerable<MovieImport> movieImports, Realm realm)
        {
            var categories = movieImports.Select(m => m.Categories.GetNonEmptyListFromCommaSeparatedValues())
                                         .SelectMany(c => c)
                                         .ToHashSet();
            await realm.WriteAsync(r =>
            {
                foreach(string categoryName in categories)
                {
                    var category = r.All<Category>().FirstOrDefault(c => c.Name == categoryName);
                    if(category == null)
                    {
                        category = r.Add(new Category
                                         {
                                             Id = ObjectId.GenerateNewId(),
                                             Name = categoryName,
                                             PartitionKey = DashboardPartitionKey
                                         });
                    }

                    Categories[categoryName] = category;
                }
            });
        }

        private static async Task AddAllGenres(IEnumerable<MovieImport> movieImports, Realm realm)
        {
            var genres = movieImports.Select(m => m.Genres.GetNonEmptyListFromCommaSeparatedValues())
                                     .SelectMany(g => g)
                                     .ToHashSet();
            await realm.WriteAsync(r =>
            {
                foreach(string genreName in genres)
                {
                    var genre = r.All<Genre>().FirstOrDefault(g => g.Name == genreName);
                    if(genre == null)
                    {
                        genre = r.Add(new Genre
                                      {
                                          Id = ObjectId.GenerateNewId(),
                                          Name = genreName,
                                          PartitionKey = DashboardPartitionKey
                                      });
                    }

                    Genres[genreName] = genre;
                }
            });
        }

        private static IEnumerable<string> GetNonEmptyListFromCommaSeparatedValues(this string commaSeparateString)
        {
            return commaSeparateString.Split(",", StringSplitOptions.RemoveEmptyEntries)
                                      .Select(p => p.Trim())
                                      .Where(p => !string.IsNullOrWhiteSpace(p));
        }
    }
}