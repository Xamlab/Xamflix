namespace Xamflix.Core.Pipeline
{
    public static class PipelineCommandExtensions
    {
        public static IPipelineCommand<TContext, TResult> ContinueWith<TContext, TResult>(this IPipelineCommand<TContext, TResult> command,
                                                                                          IPipelineCommand<TContext, TResult> next)
        {
            command.Next = next;
            return next;
        }
    }
}