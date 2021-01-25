using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Xamarin.Forms;
using Xamflix.Domain.Data;
using Xamflix.Domain.Models;

namespace Xamflix.App.Forms
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; internal set; }
        
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            Test();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private async void Test()
        {
            var movieId = ObjectId.Parse("600e52634774ab39245b8a08");
            var dbContext = Services.GetRequiredService<IAppDbContext>();
            var movie = (await dbContext.GetItemsAsync<Movie>()).FirstOrDefault(m => m.Id == movieId);
            if(movie == null)
            {
                movie = new Movie
                        {
                            Id = movieId,
                            Name = "The Midnight Sky",
                            Year = 2020,
                            DurationInSeconds = 122 * 60,
                            Synopsis = "In the aftermath of a global catastrophe, a lone scientist in the Arctic races to contact a crew of astronauts with a warning not to return to Earth."
                        };
                await dbContext.RunInTransactionAsync(async tx =>
                {
                    await dbContext.AddAsync(movie, tx);
                });
            }
        }
    }
}