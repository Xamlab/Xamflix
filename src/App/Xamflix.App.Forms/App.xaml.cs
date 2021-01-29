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
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}