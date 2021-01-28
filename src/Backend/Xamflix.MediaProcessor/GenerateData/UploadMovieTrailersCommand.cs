using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using Xamflix.Core.Pipeline;
using Xamflix.Domain.Data.Realm;
using Xamflix.Domain.Models;
using Xamflix.MediaProcessor.Models;
using Xamflix.MediaProcessor.Services;

namespace Xamflix.MediaProcessor.GenerateData
{
    public class UploadMovieTrailersCommand : IPipelineCommand<GenerateDataContext, GenerateDataResult>
    {
        private readonly IRealmFactory _realmFactory;
        private readonly IMediaService _mediaService;

        public UploadMovieTrailersCommand(IRealmFactory realmFactory,
                                          IMediaService mediaService)
        {
            _realmFactory = realmFactory;
            _mediaService = mediaService;
        }

        public IPipelineCommand<GenerateDataContext, GenerateDataResult> Next { get; set; }

        public async Task<GenerateDataResult> ExecuteAsync(GenerateDataContext context, CancellationToken token = default)
        {
            Console.WriteLine("Uploading movie trailers");
            var uploadTasks = context.MovieImports
                                     .Select(import => new
                                                       {
                                                           MovieImport = import,
                                                           MovieId = context.Movies[import]
                                                       })
                                     .Select(import => UploadMovieTrailerAsync(context, import.MovieId, import.MovieImport));
            var taskResults = await Task.WhenAll(uploadTasks);
            var failedTasks = taskResults.Where(t => !t.IsSuccessful).ToArray();

            if(failedTasks.Any())
            {
                var exception = new AggregateException(failedTasks
                                                           .Select(taskResult => new Exception($"Upload movie trailer task {taskResult.TaskName} failed.", taskResult.Error)));
                return new GenerateDataResult("Something went wrong while uploading movie trailers", exception);
            }

            Console.WriteLine("Movie trailers successfully uploaded");
            return new GenerateDataResult(true);
        }

        private async Task<TaskResult<bool>> UploadMovieTrailerAsync(GenerateDataContext context, ObjectId movieId, MovieImport movieImport)
        {
            Console.WriteLine($"Uploading movie trailer {movieId}");

            var mediaName = $"trailer-{movieId}";
            TaskResult<bool> taskResult = new()
                                          {
                                              TaskName = mediaName
                                          };

            if(await CheckIfTrailerAlreadyExistsAsync(movieId))
            {
                Console.WriteLine($"Movie trailer for {movieId} has already been uploaded");
                taskResult.IsSuccessful = true;
                return taskResult;
            }

            try
            {
                var trailFilePath = Path.Combine(context.ImportRootDir, "Trailers", movieImport.TrailerFileName);
                var streamingUrls = await _mediaService.EncodeVideoForStreaming(trailFilePath, mediaName);
                using var realm = await _realmFactory.GetDefaultSyncedRealmAsync();
                await realm.WriteAsync(r =>
                {
                    var movie = r.Find<Movie>(movieId);
                    movie.StreamingUrl = streamingUrls.First(url => url.Contains("m3u8-aapl"));
                });
                taskResult.IsSuccessful = true;
                taskResult.Result = true;
                Console.WriteLine($"Successfully uploaded movie trailer for {movieId}");
            }
            catch(Exception ex)
            {
                taskResult.IsSuccessful = false;
                taskResult.Error = ex;
            }

            return taskResult;
        }

        private async Task<bool> CheckIfTrailerAlreadyExistsAsync(ObjectId movieId)
        {
            using var realm = await _realmFactory.GetDefaultSyncedRealmAsync();

            var movie = realm.Find<Movie>(movieId);
            return !string.IsNullOrWhiteSpace(movie.StreamingUrl);
        }
    }
}