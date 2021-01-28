using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xamflix.MediaProcessor.Services
{
    public interface IMediaService
    {
        Task<IEnumerable<string>> EncodeVideoForStreaming(string inputMp4FileName, 
                                                          string uniqueMediaName, 
                                                          string downloadResultAssetsOutputPath = null);
    }
}