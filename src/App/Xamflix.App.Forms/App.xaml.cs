using System;
using Xamflix.App.Forms.Pages.Dashboard;
using Xamflix.Core.AsyncVoid;

namespace Xamflix.App.Forms
{
    public partial class App
    {
        public static IServiceProvider Services { get; internal set; } = null!;

        public App()
        {
            InitializeComponent();

            MainPage = new DashboardPage();
        }

        protected override void OnStart()
        {
#if DEBUG
            typeof(App).Assembly.AssertNoAsyncVoidMethods();
            typeof(ViewModels.Bootstrapper).Assembly.AssertNoAsyncVoidMethods();
#endif
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}