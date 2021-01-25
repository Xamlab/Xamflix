namespace Xamflix.Core.Services
{
    public interface ISystemPathService
    {
        string GetLocalPath(string fileOrFolderName);
        string GetTempPath(string fileOrFolderName);
    }
}