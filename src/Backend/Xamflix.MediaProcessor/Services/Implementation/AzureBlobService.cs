using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Xamflix.MediaProcessor.Configuration;

namespace Xamflix.MediaProcessor.Services.Implementation
{
    public class AzureBlobService : IBlobService
    {
        private readonly BlobServiceClient _blobClient;

        public AzureBlobService(BlobStorageConfiguration configuration)
        {
            _blobClient = new BlobServiceClient(configuration.ConnectionString);
        }

        public async Task<string> UploadFileAsync(string filePath, string container, string blobName, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            BlobContainerClient containerClient = _blobClient.GetBlobContainerClient(container);
            // Create the container if it does not exist.
            await containerClient.CreateIfNotExistsAsync(cancellationToken:cancellationToken);

            await using var stream = File.OpenRead(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            if(overwrite || !await blobClient.ExistsAsync(cancellationToken))
            {
                await blobClient.UploadAsync(stream, overwrite, cancellationToken);   
            }
            return blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.MaxValue).ToString();
        }
    }
}