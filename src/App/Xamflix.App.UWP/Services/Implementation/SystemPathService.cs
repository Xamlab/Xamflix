using System.IO;
using Windows.Storage;
using Xamflix.Core.Services;

namespace Xamflix.App.UWP.Services.Implementation
{
    public class SystemPathService : ISystemPathService
    {
        public string GetLocalPath(string fileOrFolderName)
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, fileOrFolderName);
        }

        public string GetTempPath(string fileOrFolderName)
        {
            return ApplicationData.Current.TemporaryFolder.Path;
        }
    }
}