using System;
using System.IO;
using Foundation;
using Xamflix.Core.Services;

namespace Xamflix.App.iOS.Services.Implementation
{
    public class SystemPathService : ISystemPathService
    {
        public string GetLocalPath(string fileOrFolderName)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library");
            string fileOrFolder = Path.Combine(libFolder, fileOrFolderName);
            return fileOrFolder;
        }

        public string GetTempPath(string fileOrFolderName)
        {
            string documents = NSFileManager.DefaultManager
                                            .GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;

            string tempFolderPath = Path.Combine(documents, "../", "tmp");
            return tempFolderPath;
        }
    }
}