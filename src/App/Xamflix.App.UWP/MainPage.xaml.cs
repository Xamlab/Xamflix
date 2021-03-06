﻿using MediaManager;
using Xamflix.App.Forms;

namespace Xamflix.App.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            
            Forms.Bootstrapper.CreateContainer()
                 .RegisterFormsDependencies()
                 .AddUwp()
                 .BuildContainer();
            
            CrossMediaManager.Current.Init();
            LoadApplication(new Forms.App());
        }
    }
}