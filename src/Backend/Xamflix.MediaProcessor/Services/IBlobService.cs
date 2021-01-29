using System.Threading;
using System.Threading.Tasks;

namespace Xamflix.MediaProcessor.Services
{
    public interface IBlobService
    {
        Task<string> UploadFileAsync(string filePath, string container, string blobName, bool overwrite = false, CancellationToken cancellationToken = default);
    }
}