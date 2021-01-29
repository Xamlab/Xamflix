using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Realms;

namespace Xamflix.Domain.Data
{
    public interface IAppDbContext : IDisposable
    {
        Task<IQueryable<T>> GetItemsAsync<T>() where T : RealmObject;
        Task<ITransaction?> StartTransactionAsync();
        Task<T> AddAsync<T>(T item, ITransaction transaction) where T : RealmObject;
        Task UpdateItemAsync<T>(string id, Action<T> updateAction, ITransaction transaction) where T : RealmObject;
        Task UpdateItemAsync<T>(string id, Func<T, Task> updateAction, ITransaction transaction) where T : RealmObject;
        Task RemoveAsync<T>(T item, ITransaction transaction) where T : RealmObject;
        Task RemoveRangeAsync<T>(IQueryable<T> items, ITransaction transaction) where T : RealmObject;
        Task<bool> SynchronizeAsync(CancellationToken token = default);
    }
}