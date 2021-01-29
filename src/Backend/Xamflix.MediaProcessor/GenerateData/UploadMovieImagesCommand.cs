using System;
using System.Collections.Generic;
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
    public class UploadMovieImagesCommand : IPipelineCommand<GenerateDataContext, GenerateDataResult>
    {
        private readonly IRealmFactory _realmFactory;
        private readonly IBlobService _blobService;

        public UploadMovieImagesCommand(IRealmFactory realmFactory,
                                        IBlobService blobService)
        {
            _realmFactory = realmFactory;
            _blobService = blobService;
        }

        public IPipelineCommand<GenerateDataContext, GenerateDataResult> Next { get; set; } = null!;

        public async Task<GenerateDataResult> ExecuteAsync(GenerateDataContext context, CancellationToken token = default)
        {
            Console.WriteLine("Uploading movie images");
            var uploadTasks = new List<Task<TaskResult<string>>>();
            using var realm = await _realmFactory.GetDefaultSyncedRealmAsync();
            foreach(MovieImport movieImport in context.MovieImports)
            {
                var movie = realm.Find<Movie>(context.Movies[movieImport]);
                uploadTasks.AddRange(UploadMovieImage(movieImport, movie, context));
            }

            var taskResults = await Task.WhenAll(uploadTasks);
            var failedTasks = taskResults.Where(t => !t.IsSuccessful).ToArray();
            if(failedTasks.Any())
            {
                var exception = new AggregateException(failedTasks
                                                           .Select(taskResult => new Exception($"Upload movie image task {taskResult.TaskName} failed", taskResult.Error)));
                return new GenerateDataResult("Something went wrong while uploading movie images", exception);
            }

            Console.WriteLine("Movie images were successfully uploaded");
            return await Next.ExecuteAsync(context, token);
        }

        private IEnumerable<Task<TaskResult<string>>> UploadMovieImage(MovieImport movieImport,
                                                                       Movie movie,
                                                                       GenerateDataContext context)
        {
            var uploadTasks = new List<Task<TaskResult<string>>>();

            if(string.IsNullOrWhiteSpace(movie.PosterImageUrl) && !string.IsNullOrWhiteSpace(movieImport.PosterImageFileName))
            {
                uploadTasks.Add(UploadImage(movie.Id,
                                            "Posters",
                                            movieImport.PosterImageFileName,
                                            (imageUrl, m) => m.PosterImageUrl = imageUrl,
                                            context));
            }

            if(string.IsNullOrWhiteSpace(movie.PosterTitleImageUrl) && !string.IsNullOrWhiteSpace(movieImport.PosterTitleImageFileName))
            {
                uploadTasks.Add(UploadImage(movie.Id,
                                            "PosterTitles",
                                            movieImport.PosterTitleImageFileName,
                                            (imageUrl, m) => m.PosterTitleImageUrl = imageUrl,
                                            context));
            }

            if(string.IsNullOrWhiteSpace(movie.ThumbnailImageUrl) && !string.IsNullOrWhiteSpace(movieImport.ThumbnailImageFileName))
            {
                uploadTasks.Add(UploadImage(movie.Id,
                                            "Thumbnails",
                                            movieImport.ThumbnailImageFileName,
                                            (imageUrl, m) => m.ThumbnailImageUrl = imageUrl,
                                            context));
            }

            if(string.IsNullOrWhiteSpace(movie.TallThumbnailImageUrl) && !string.IsNullOrWhiteSpace(movieImport.TallThumbnailFileName))
            {
                uploadTasks.Add(UploadImage(movie.Id,
                                            "TallThumbnails",
                                            movieImport.TallThumbnailFileName,
                                            (imageUrl, m) => m.TallThumbnailImageUrl = imageUrl,
                                            context));
            }
            
            if(string.IsNullOrWhiteSpace(movie.BillboardPosterImageUrl) && !string.IsNullOrWhiteSpace(movieImport.BillboardPosterImageFileName))
            {
                uploadTasks.Add(UploadImage(movie.Id,
                                            "BillboardPosters",
                                            movieImport.BillboardPosterImageFileName,
                                            (imageUrl, m) => m.BillboardPosterImageUrl = imageUrl,
                                            context));
            }
            
            if(string.IsNullOrWhiteSpace(movie.BillboardPosterTitleImageUrl) && !string.IsNullOrWhiteSpace(movieImport.BillboardPosterTitleImageFileName))
            {
                uploadTasks.Add(UploadImage(movie.Id,
                                            "BillboardPosterTitles",
                                            movieImport.BillboardPosterTitleImageFileName,
                                            (imageUrl, m) => m.BillboardPosterTitleImageUrl = imageUrl,
                                            context));
            }

            return uploadTasks;
        }

        private async Task<TaskResult<string>> UploadImage(ObjectId movieId,
                                                           string imageType,
                                                           string imageFileName,
                                                           Action<string, Movie> assignImageUrl, 
                                                           GenerateDataContext context)
        {
            Console.WriteLine($"Uploading movie image {imageType}/{imageFileName}");
            TaskResult<string> result = new()
                                        {
                                            TaskName = $"{movieId}-{imageType}-{imageFileName}",
                                            IsSuccessful = true
                                        };
            try
            {
                string imageFilePath = Path.Combine(context.ImportRootDir, imageType, imageFileName);
                var blobName = $"{movieId}/{imageType}/{imageFileName}";
                string imageUrl = await _blobService.UploadFileAsync(imageFilePath, "movieimages", blobName);
                using var realm = await _realmFactory.GetDefaultSyncedRealmAsync();
                await realm.WriteAsync(r =>
                {
                    var m = r.Find<Movie>(movieId);
                    assignImageUrl(imageUrl, m);
                });
                result.IsSuccessful = true;
                result.Result = imageUrl;
                Console.WriteLine($"Successfully uploaded movie image {imageType}/{imageFileName}");
            }
            catch(Exception ex)
            {
                result.IsSuccessful = false;
                result.Error = ex;
            }

            return result;
        }
    }
}