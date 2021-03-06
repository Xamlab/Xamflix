﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using Xamflix.MediaProcessor.Configuration;

namespace Xamflix.MediaProcessor.Services.Implementation
{
    public class MediaService : IMediaService
    {
        private readonly MediaServiceConfiguration _configuration;
        private const string AdaptiveStreamingTransformName = "XamflixTransformWithAdaptiveStreamingPreset";

        public MediaService(MediaServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<string>> EncodeVideoForStreaming(string inputMp4FileName, string uniqueMediaName, string downloadResultAssetsOutputPath = null)
        {
            var client = await CreateMediaServicesClientAsync(_configuration);

            // Set the polling interval for long running operations to 2 seconds.
            // The default value is 30 seconds for the .NET client SDK
            client.LongRunningOperationRetryTimeout = 2;

            // Creating a unique suffix so that we don't have name collisions if you run the sample
            // multiple times without cleaning up.
            var jobName = $"job-{uniqueMediaName}";
            var locatorName = $"locator-{uniqueMediaName}";
            var outputAssetName = $"{uniqueMediaName}";
            var inputAssetName = $"input-{uniqueMediaName}";

            var existingStreamingUrls = await GetExistingStreamingUrls(locatorName,
                                                                       _configuration.ResourceGroup,
                                                                       _configuration.AccountName,
                                                                       client);
            if(existingStreamingUrls?.Any() == true)
            {
                return existingStreamingUrls;
            }

            // Ensure that you have the desired encoding Transform. This is really a one time setup operation.
            _ = await GetOrCreateTransformAsync(client, _configuration.ResourceGroup, _configuration.AccountName,
                                                AdaptiveStreamingTransformName);

            // Create a new input Asset and upload the specified local video file into it.
            _ = await CreateInputAssetAsync(client, _configuration.ResourceGroup, _configuration.AccountName, inputAssetName,
                                            inputMp4FileName);

            // Use the name of the created input asset to create the job input.
            _ = new JobInputAsset(inputAssetName);

            // Output from the encoding Job must be written to an Asset, so let's create one
            var outputAsset =
                await CreateOutputAssetAsync(client, _configuration.ResourceGroup, _configuration.AccountName, outputAssetName);

            _ = await SubmitJobAsync(client, _configuration.ResourceGroup, _configuration.AccountName, AdaptiveStreamingTransformName,
                                     jobName, inputAssetName, outputAsset.Name);
            // In this demo code, we will poll for Job status
            // Polling is not a recommended best practice for production applications because of the latency it introduces.
            // Overuse of this API may trigger throttling. Developers should instead use Event Grid.
            var job = await WaitForJobToFinishAsync(client, _configuration.ResourceGroup, _configuration.AccountName,
                                                    AdaptiveStreamingTransformName, jobName);

            if(job.State == JobState.Finished)
            {
                Console.WriteLine("Job finished. Cleaning up resources.");

                await CleanUpAsync(client,
                                   _configuration.ResourceGroup,
                                   _configuration.AccountName,
                                   AdaptiveStreamingTransformName,
                                   jobName,
                                   new List<string> {inputAssetName});

                if(!string.IsNullOrWhiteSpace(downloadResultAssetsOutputPath))
                {
                    if(!Directory.Exists(downloadResultAssetsOutputPath))
                        Directory.CreateDirectory(downloadResultAssetsOutputPath);

                    await DownloadOutputAssetAsync(client,
                                                   _configuration.ResourceGroup,
                                                   _configuration.AccountName,
                                                   outputAsset.Name,
                                                   downloadResultAssetsOutputPath);
                }

                var locator = await CreateStreamingLocatorAsync(client,
                                                                _configuration.ResourceGroup,
                                                                _configuration.AccountName,
                                                                outputAsset.Name,
                                                                locatorName);

                // Note that the URLs returned by this method include a /manifest path followed by a (format=)
                // parameter that controls the type of manifest that is returned. 
                // The /manifest(format=m3u8-aapl) will provide Apple HLS v4 manifest using MPEG TS segments.
                // The /manifest(format=mpd-time-csf) will provide MPEG DASH manifest.
                // And using just /manifest alone will return Microsoft Smooth Streaming format.
                // There are additional formats available that are not returned in this call, please check the documentation
                // on the dynamic packager for additional formats - see https://docs.microsoft.com/azure/media-services/latest/dynamic-packaging-overview
                var urls = await GetStreamingUrlsAsync(client, _configuration.ResourceGroup, _configuration.AccountName, locator.Name);
                return urls;
            }

            throw new Exception($"Failed to upload video {inputAssetName}");
        }

        private async Task<IList<string>> GetExistingStreamingUrls(string locatorName,
                                                                   string resourceGroup,
                                                                   string accountName,
                                                                   IAzureMediaServicesClient client)
        {
            var locator = await client.StreamingLocators.GetAsync(resourceGroup, accountName, locatorName);
            if(locator != null)
            {
                var urls = await GetStreamingUrlsAsync(client, _configuration.ResourceGroup, _configuration.AccountName, locator.Name);
                return urls;
            }

            return null;
        }

        /// <summary>
        ///     Create the ServiceClientCredentials object based on the credentials
        ///     supplied in local configuration file.
        /// </summary>
        /// <param name="_configuration">
        ///     The param is of type <see cref="MediaServiceConfiguration" />. This class reads values from local
        ///     configuration file.
        /// </param>
        /// <returns></returns>
        // <GetCredentialsAsync>
        private static async Task<ServiceClientCredentials> GetCredentialsAsync(MediaServiceConfiguration _configuration)
        {
            // Use ApplicationTokenProvider.LoginSilentWithCertificateAsync or UserTokenProvider.LoginSilentAsync to get a token using service principal with certificate
            //// ClientAssertionCertificate
            //// ApplicationTokenProvider.LoginSilentWithCertificateAsync

            // Use ApplicationTokenProvider.LoginSilentAsync to get a token using a service principal with symetric key
            var clientCredential = new ClientCredential(_configuration.AadClientId, _configuration.AadSecret);
            return await ApplicationTokenProvider.LoginSilentAsync(_configuration.AadTenantId, clientCredential,
                                                                   ActiveDirectoryServiceSettings.Azure);
        }
        // </GetCredentialsAsync>

        /// <summary>
        ///     Creates the AzureMediaServicesClient object based on the credentials
        ///     supplied in local configuration file.
        /// </summary>
        /// <param name="configuration">
        ///     The param is of type <see cref="MediaServiceConfiguration" />. This class reads values from local
        ///     configuration file.
        /// </param>
        /// <returns></returns>
        // <CreateMediaServicesClient>
        private static async Task<IAzureMediaServicesClient> CreateMediaServicesClientAsync(
            MediaServiceConfiguration configuration)
        {
            var credentials = await GetCredentialsAsync(configuration);

            return new AzureMediaServicesClient(configuration.ArmEndpoint, credentials)
                   {
                       SubscriptionId = configuration.SubscriptionId
                   };
        }
        // </CreateMediaServicesClient>

        /// <summary>
        ///     Creates a new input Asset and uploads the specified local video file into it.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="assetName">The asset name.</param>
        /// <param name="fileToUpload">The file you want to upload into the asset.</param>
        /// <returns></returns>
        // <CreateInputAsset>
        private static async Task<Asset> CreateInputAssetAsync(
            IAzureMediaServicesClient client,
            string resourceGroupName,
            string accountName,
            string assetName,
            string fileToUpload)
        {
            // In this example, we are assuming that the asset name is unique.
            //
            // If you already have an asset with the desired name, use the Assets.Get method
            // to get the existing asset. In Media Services v3, the Get method on entities returns null 
            // if the entity doesn't exist (a case-insensitive check on the name).

            // Call Media Services API to create an Asset.
            // This method creates a container in storage for the Asset.
            // The files (blobs) associated with the asset will be stored in this container.
            var asset = await client.Assets.CreateOrUpdateAsync(resourceGroupName, accountName, assetName, new Asset());

            // Use Media Services API to get back a response that contains
            // SAS URL for the Asset container into which to upload blobs.
            // That is where you would specify read-write permissions 
            // and the expiration time for the SAS URL.
            var response = await client.Assets.ListContainerSasAsync(
                                                                     resourceGroupName,
                                                                     accountName,
                                                                     assetName,
                                                                     AssetContainerPermission.ReadWrite,
                                                                     DateTime.UtcNow.AddHours(4).ToUniversalTime());

            var sasUri = new Uri(response.AssetContainerSasUrls.First());

            // Use Storage API to get a reference to the Asset container
            // that was created by calling Asset's CreateOrUpdate method.  
            var container = new BlobContainerClient(sasUri);
            var blob = container.GetBlobClient(Path.GetFileName(fileToUpload));

            // Use Storage API to upload the file into the container in storage.
            await blob.UploadAsync(fileToUpload);

            return asset;
        }
        // </CreateInputAsset>

        /// <summary>
        ///     Creates an output asset. The output from the encoding Job must be written to an Asset.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="assetName">The output asset name.</param>
        /// <returns></returns>
        // <CreateOutputAsset>
        private static async Task<Asset> CreateOutputAssetAsync(IAzureMediaServicesClient client,
                                                                string resourceGroupName, string accountName, string assetName)
        {
            // Check if an Asset already exists
            var outputAsset = await client.Assets.GetAsync(resourceGroupName, accountName, assetName);
            var asset = new Asset();
            var outputAssetName = assetName;

            if(outputAsset != null)
            {
                // Name collision! In order to get the sample to work, let's just go ahead and create a unique asset name
                // Note that the returned Asset can have a different name than the one specified as an input parameter.
                // You may want to update this part to throw an Exception instead, and handle name collisions differently.
                var uniqueness = $"-{Guid.NewGuid():N}";
                outputAssetName += uniqueness;

                Console.WriteLine("Warning – found an existing Asset with name = " + assetName);
                Console.WriteLine("Creating an Asset with this name instead: " + outputAssetName);
            }

            return await client.Assets.CreateOrUpdateAsync(resourceGroupName, accountName, outputAssetName, asset);
        }
        // </CreateOutputAsset>

        /// <summary>
        ///     If the specified transform exists, get that transform.
        ///     If the it does not exist, creates a new transform with the specified output.
        ///     In this case, the output is set to encode a video using one of the built-in encoding presets.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="transformName">The name of the transform.</param>
        /// <returns></returns>
        // <EnsureTransformExists>
        private static async Task<Transform> GetOrCreateTransformAsync(
            IAzureMediaServicesClient client,
            string resourceGroupName,
            string accountName,
            string transformName)
        {
            // Does a Transform already exist with the desired name? Assume that an existing Transform with the desired name
            // also uses the same recipe or Preset for processing content.
            var transform = await client.Transforms.GetAsync(resourceGroupName, accountName, transformName);

            if(transform == null)
            {
                // You need to specify what you want it to produce as an output
                TransformOutput[] output =
                {
                    new TransformOutput
                    {
                        // The preset for the Transform is set to one of Media Services built-in sample presets.
                        // You can  customize the encoding settings by changing this to use "StandardEncoderPreset" class.
                        Preset = new BuiltInStandardEncoderPreset
                                 {
                                     // This sample uses the built-in encoding preset for Adaptive Bitrate Streaming.
                                     PresetName = EncoderNamedPreset.AdaptiveStreaming
                                 }
                    }
                };

                // Create the Transform with the output defined above
                transform = await client.Transforms.CreateOrUpdateAsync(resourceGroupName, accountName, transformName,
                                                                        output);
            }

            return transform;
        }
        // </EnsureTransformExists>

        /// <summary>
        ///     Submits a request to Media Services to apply the specified Transform to a given input video.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="transformName">The name of the transform.</param>
        /// <param name="jobName">The (unique) name of the job.</param>
        /// <param name="inputAssetName">The name of the input asset.</param>
        /// <param name="outputAssetName">The (unique) name of the  output asset that will store the result of the encoding job. </param>
        // <SubmitJob>
        private static async Task<Job> SubmitJobAsync(IAzureMediaServicesClient client,
                                                      string resourceGroupName,
                                                      string accountName,
                                                      string transformName,
                                                      string jobName,
                                                      string inputAssetName,
                                                      string outputAssetName)
        {
            // Use the name of the created input asset to create the job input.
            JobInput jobInput = new JobInputAsset(inputAssetName);

            JobOutput[] jobOutputs =
            {
                new JobOutputAsset(outputAssetName)
            };

            // In this example, we are assuming that the job name is unique.
            //
            // If you already have a job with the desired name, use the Jobs.Get method
            // to get the existing job. In Media Services v3, the Get method on entities returns null 
            // if the entity doesn't exist (a case-insensitive check on the name).
            var job = await client.Jobs.GetAsync(resourceGroupName, accountName, transformName, jobName)
                      ?? await client.Jobs.CreateAsync(
                                                       resourceGroupName,
                                                       accountName,
                                                       transformName,
                                                       jobName,
                                                       new Job
                                                       {
                                                           Input = jobInput,
                                                           Outputs = jobOutputs
                                                       });
            return job;
        }
        // </SubmitJob>

        /// <summary>
        ///     Polls Media Services for the status of the Job.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="transformName">The name of the transform.</param>
        /// <param name="jobName">The name of the job you submitted.</param>
        /// <returns></returns>
        // <WaitForJobToFinish>
        private static async Task<Job> WaitForJobToFinishAsync(IAzureMediaServicesClient client,
                                                               string resourceGroupName,
                                                               string accountName,
                                                               string transformName,
                                                               string jobName)
        {
            const int sleepIntervalMs = 20 * 1000;

            Job job;
            do
            {
                job = await client.Jobs.GetAsync(resourceGroupName, accountName, transformName, jobName);

                Console.WriteLine($"{jobName} is '{job.State}'.");
                for(var i = 0; i < job.Outputs.Count; i++)
                {
                    var output = job.Outputs[i];
                    Console.Write($"\t {jobName}.JobOutput[{i}] is '{output.State}'.");
                    if(output.State == JobState.Processing) Console.Write($"  Progress (%): '{output.Progress}'.");

                    Console.WriteLine();
                }

                if(job.State != JobState.Finished && job.State != JobState.Error && job.State != JobState.Canceled)
                    await Task.Delay(sleepIntervalMs);
            } while(job.State != JobState.Finished && job.State != JobState.Error && job.State != JobState.Canceled);

            return job;
        }
        // </WaitForJobToFinish>

        /// <summary>
        ///     Creates a StreamingLocator for the specified asset and with the specified streaming policy name.
        ///     Once the StreamingLocator is created the output asset is available to clients for playback.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroup">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="assetName">The name of the output asset.</param>
        /// <param name="locatorName">The StreamingLocator name (unique in this case).</param>
        /// <returns></returns>
        // <CreateStreamingLocator>
        private static async Task<StreamingLocator> CreateStreamingLocatorAsync(
            IAzureMediaServicesClient client,
            string resourceGroup,
            string accountName,
            string assetName,
            string locatorName)
        {
            var locator = await client.StreamingLocators.CreateAsync(
                                                                     resourceGroup,
                                                                     accountName,
                                                                     locatorName,
                                                                     new StreamingLocator
                                                                     {
                                                                         AssetName = assetName,
                                                                         StreamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly
                                                                     });

            return locator;
        }
        // </CreateStreamingLocator>

        /// <summary>
        ///     Checks if the "default" streaming endpoint is in the running state,
        ///     if not, starts it.
        ///     Then, builds the streaming URLs.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroupName">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="locatorName">The name of the StreamingLocator that was created.</param>
        /// <returns></returns>
        // <GetStreamingURLs>
        private static async Task<IList<string>> GetStreamingUrlsAsync(
            IAzureMediaServicesClient client,
            string resourceGroupName,
            string accountName,
            string locatorName)
        {
            const string defaultStreamingEndpointName = "default";

            IList<string> streamingUrls = new List<string>();

            var streamingEndpoint =
                await client.StreamingEndpoints.GetAsync(resourceGroupName, accountName, defaultStreamingEndpointName);

            if(streamingEndpoint != null)
                if(streamingEndpoint.ResourceState != StreamingEndpointResourceState.Running)
                    await client.StreamingEndpoints.StartAsync(resourceGroupName, accountName,
                                                               defaultStreamingEndpointName);

            var paths = await client.StreamingLocators.ListPathsAsync(resourceGroupName, accountName, locatorName);

            foreach(var path in paths.StreamingPaths)
            {
                var uriBuilder = new UriBuilder
                                 {
                                     Scheme = "https",
                                     Host = streamingEndpoint.HostName,

                                     Path = path.Paths[0]
                                 };
                streamingUrls.Add(uriBuilder.ToString());
            }

            return streamingUrls;
        }
        // </GetStreamingURLs>

        /// <summary>
        ///     Downloads the results from the specified output asset, so you can see what you got.
        /// </summary>
        /// <param name="client">The Media Services client.</param>
        /// <param name="resourceGroup">The name of the resource group within the Azure subscription.</param>
        /// <param name="accountName"> The Media Services account name.</param>
        /// <param name="assetName">The output asset.</param>
        /// <param name="outputFolderName">The name of the folder into which to download the results.</param>
        // <DownloadResults>
        private static async Task DownloadOutputAssetAsync(
            IAzureMediaServicesClient client,
            string resourceGroup,
            string accountName,
            string assetName,
            string outputFolderName)
        {
            if(!Directory.Exists(outputFolderName)) Directory.CreateDirectory(outputFolderName);

            var assetContainerSas = await client.Assets.ListContainerSasAsync(
                                                                              resourceGroup,
                                                                              accountName,
                                                                              assetName,
                                                                              AssetContainerPermission.Read,
                                                                              DateTime.UtcNow.AddHours(1).ToUniversalTime());

            var containerSasUrl = new Uri(assetContainerSas.AssetContainerSasUrls.FirstOrDefault());
            var container = new BlobContainerClient(containerSasUrl);

            var directory = Path.Combine(outputFolderName, assetName);
            Directory.CreateDirectory(directory);

            Console.WriteLine($"Downloading output results to '{directory}'...");

            string continuationToken = null;
            IList<Task> downloadTasks = new List<Task>();

            do
            {
                var resultSegment = container.GetBlobs().AsPages(continuationToken);

                foreach(var blobPage in resultSegment)
                {
                    foreach(var blobItem in blobPage.Values)
                    {
                        var blobClient = container.GetBlobClient(blobItem.Name);
                        var filename = Path.Combine(directory, blobItem.Name);

                        downloadTasks.Add(blobClient.DownloadToAsync(filename));
                    }

                    // Get the continuation token and loop until it is empty.
                    continuationToken = blobPage.ContinuationToken;
                }
            } while(continuationToken != "");

            await Task.WhenAll(downloadTasks);

            Console.WriteLine("Download complete.");
        }
        // </DownloadResults>

        /// <summary>
        ///     Deletes the jobs, assets and potentially the content key policy that were created.
        ///     Generally, you should clean up everything except objects
        ///     that you are planning to reuse (typically, you will reuse Transforms, and you will persist output assets and
        ///     StreamingLocators).
        /// </summary>
        /// <param name="client"></param>
        /// <param name="resourceGroupName"></param>
        /// <param name="accountName"></param>
        /// <param name="transformName"></param>
        /// <param name="jobName"></param>
        /// <param name="assetNames"></param>
        /// <param name="contentKeyPolicyName"></param>
        /// <returns></returns>
        // <CleanUp>
        private static async Task CleanUpAsync(
            IAzureMediaServicesClient client,
            string resourceGroupName,
            string accountName,
            string transformName,
            string jobName,
            List<string> assetNames,
            string contentKeyPolicyName = null
        )
        {
            await client.Jobs.DeleteAsync(resourceGroupName, accountName, transformName, jobName);

            foreach(var assetName in assetNames)
                await client.Assets.DeleteAsync(resourceGroupName, accountName, assetName);

            if(contentKeyPolicyName != null)
                await client.ContentKeyPolicies.DeleteAsync(resourceGroupName, accountName, contentKeyPolicyName);
        }

        // </CleanUp>
    }
}