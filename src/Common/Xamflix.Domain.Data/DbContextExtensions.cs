using System;
using System.Threading.Tasks;

namespace Xamflix.Domain.Data
{
    public static class DbContextExtensions
    {
        public static async Task RunInTransactionAsync(this IAppDbContext dbContext, Action<ITransaction> action, ITransaction? transaction = null)
        {
            //We should commit and dispose the transaction only if we create it ourselves, that is it's an inbound transaction
            //if the transaction is provided by the caller, it's the responsibility of the caller to commit and dispose it
            bool shouldCreateInboundTransaction = transaction == null;
            transaction ??= await dbContext.StartTransactionAsync();
            bool outboundTransactionExists = transaction == null;
            bool isInboundTransaction = shouldCreateInboundTransaction && !outboundTransactionExists;
            try
            {
                action(transaction!);
                if(isInboundTransaction)
                {
                    await transaction!.CommitAsync();
                }
            }
            finally
            {
                if(isInboundTransaction)
                {
                    transaction!.Dispose();
                }
            }
        }

        public static async Task RunInTransactionAsync(this IAppDbContext dbContext, Func<ITransaction, Task> action, ITransaction? transaction = null)
        {
            //We should commit and dispose the transaction only if we create it ourselves
            //if the transaction is provided by the caller, it's the responsibility of the caller to commit and dispose it
            bool shouldCreateInboundTransaction = transaction == null;
            transaction ??= await dbContext.StartTransactionAsync();
            bool outboundTransactionExists = transaction == null;
            bool isInboundTransaction = shouldCreateInboundTransaction && !outboundTransactionExists;
            try
            {
                await action(transaction!);
                if(isInboundTransaction)
                {
                    await transaction!.CommitAsync();
                }
            }
            finally
            {
                if(isInboundTransaction)
                {
                    transaction!.Dispose();
                }
            }
        }
    }
}