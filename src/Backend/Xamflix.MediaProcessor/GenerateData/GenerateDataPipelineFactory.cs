using Xamflix.Core.Pipeline;

namespace Xamflix.MediaProcessor.GenerateData
{
    public class GenerateDataPipelineFactory
    {
        private readonly RefreshRealmCommand _refreshRealmCommand;
        private readonly LoadMovieImportsCommand _loadMovieImportsCommand;
        private readonly GeneratePeopleCommand _generatePeopleCommand;
        private readonly GenerateCategoriesCommand _generateCategoriesCommand;
        private readonly GenerateGenresCommand _generateGenresCommand;
        private readonly GenerateMoviesCommand _generateMoviesCommand;
        private readonly UploadMovieImagesCommand _uploadMovieImagesCommand;
        private readonly UploadMovieTrailersCommand _uploadMovieTrailersCommand;
        private readonly UpdateMoviesWithTrailersCommand _updateMoviesWithTrailersCommand;
        private readonly BuildBillboardCommand _buildBillboardCommand;
        private readonly RefreshRealmCommand _finalRefreshRealmCommand;

        public GenerateDataPipelineFactory(RefreshRealmCommand refreshRealmCommand,
                                           LoadMovieImportsCommand loadMovieImportsCommand,
                                           GeneratePeopleCommand generatePeopleCommand,
                                           GenerateCategoriesCommand generateCategoriesCommand,
                                           GenerateGenresCommand generateGenresCommand,
                                           GenerateMoviesCommand generateMoviesCommand,
                                           UploadMovieImagesCommand uploadMovieImagesCommand,
                                           UploadMovieTrailersCommand uploadMovieTrailersCommand,
                                           UpdateMoviesWithTrailersCommand updateMoviesWithTrailersCommand,
                                           BuildBillboardCommand buildBillboardCommand,
                                           RefreshRealmCommand finalRefreshRealmCommand)
        {
            _refreshRealmCommand = refreshRealmCommand;
            _loadMovieImportsCommand = loadMovieImportsCommand;
            _generatePeopleCommand = generatePeopleCommand;
            _generateCategoriesCommand = generateCategoriesCommand;
            _generateGenresCommand = generateGenresCommand;
            _generateMoviesCommand = generateMoviesCommand;
            _uploadMovieImagesCommand = uploadMovieImagesCommand;
            _uploadMovieTrailersCommand = uploadMovieTrailersCommand;
            _updateMoviesWithTrailersCommand = updateMoviesWithTrailersCommand;
            _buildBillboardCommand = buildBillboardCommand;
            _finalRefreshRealmCommand = finalRefreshRealmCommand;
            _finalRefreshRealmCommand.IsTerminal = true;
        }

        public IPipelineCommand<GenerateDataContext, GenerateDataResult> CreateGenerateDataPipelineCommand()
        {
            _refreshRealmCommand
                .ContinueWith(_loadMovieImportsCommand)
                .ContinueWith(_generatePeopleCommand)
                .ContinueWith(_generateCategoriesCommand)
                .ContinueWith(_generateGenresCommand)
                .ContinueWith(_generateMoviesCommand)
                .ContinueWith(_uploadMovieImagesCommand)
                //.ContinueWith(_uploadMovieTrailersCommand)
                //.ContinueWith(_updateMoviesWithTrailersCommand)
                .ContinueWith(_buildBillboardCommand)
                .ContinueWith(_finalRefreshRealmCommand);
            return _refreshRealmCommand;
        }
    }
}