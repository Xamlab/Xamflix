using System.Threading.Tasks;
using Realms;

namespace Xamflix.Domain.Data.Realm.Implementation
{
    public class RealmTransaction : ITransaction
    {
        private readonly Transaction _transaction;

        public RealmTransaction(Transaction transaction)
        {
            _transaction = transaction;
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }

        public Task CommitAsync()
        {
            _transaction.Commit();
            return Task.CompletedTask;
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }
    }
}