using System.Threading;
using System.Threading.Tasks;

namespace Xamflix.Core.Pipeline
{
    public interface IPipelineCommand<TContext, TResult>
    {
        IPipelineCommand<TContext, TResult> Next { get; set; }
        Task<TResult> ExecuteAsync(TContext context, CancellationToken token = default);
    }
}