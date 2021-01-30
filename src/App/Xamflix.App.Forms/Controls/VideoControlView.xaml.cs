using System;
using MediaManager;
using Xamarin.Forms.Xaml;
using Xamflix.App.Forms.MarkupExtensions;

namespace Xamflix.App.Forms.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoControlView
    {
        public event EventHandler<EventArgs> RepeatClicked; 

        public VideoControlView()
        {
            InitializeComponent();
        }

        private void MuteButtonClicked(object sender, EventArgs e)
        {
            if (CrossMediaManager.Current.Volume.Muted)
            {
                CrossMediaManager.Current.Volume.Muted = false;
                MuteButton.ImageSource = "unMute".GetImageSource();
            }
            else
            {
                CrossMediaManager.Current.Volume.Muted = true;
                MuteButton.ImageSource = "mute".GetImageSource();
            }
        }

        private void RepeatButtonClicked(object sender, EventArgs e)
        {
            RepeatClicked(sender, e);
        }
    }
}