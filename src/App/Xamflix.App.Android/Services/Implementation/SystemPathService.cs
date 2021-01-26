using System.IO;
using Android.Content;
using Xamflix.Core.Services;

namespace Xamflix.App.Droid.Services.Implementation
{
    public class SystemPathService : ISystemPathService
    {
        private readonly Context _context;

        public SystemPathService(Context context)
        {
            _context = context;
        }
        
        public string GetLocalPath(string fileOrFolderName)
        {
            return Path.Combine(_context.FilesDir!.Path, fileOrFolderName);
        }

        public string GetTempPath(string fileOrFolderName)
        {
            return Path.Combine(_context.CacheDir!.Path, fileOrFolderName);
        }
    }
}