using MediaManager;

namespace Xamflix.App.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            CrossMediaManager.Current.Init();
            LoadApplication(new Forms.App());
        }
    }
}