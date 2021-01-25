using System;
using Android.App;
using Android.Runtime;
using Xamflix.App.Forms;

namespace Xamflix.App.Droid
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            Instance = this;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Forms.Bootstrapper.CreateContainer()
                 .RegisterFormsDependencies()
                 .RegisterAndroidDependencies()
                 .BuildContainer();
        }

        public static MainApplication Instance { get; private set; }
        
    }
}