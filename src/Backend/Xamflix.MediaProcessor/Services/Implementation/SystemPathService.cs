using System.IO;
using Xamflix.Core.Services;

namespace Xamflix.MediaProcessor.Services.Implementation
{
    public class SystemPathService : ISystemPathService
    {
        public string GetLocalPath(string fileOrFolderName)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), fileOrFolderName);
        }

        public string GetTempPath(string fileOrFolderName)
        {
            return GetLocalPath(fileOrFolderName);
        }
    }
}