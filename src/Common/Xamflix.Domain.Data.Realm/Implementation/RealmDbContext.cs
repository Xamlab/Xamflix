using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Realms;

namespace Xamflix.Domain.Data.Realm.Implementation
{
    public class RealmDbContext : IAppDbContext
    {
        private readonly IDomainServiceLocator _domainServiceLocator;
        private readonly IList<Realms.Realm> _activeRealms = new List<Realms.Realm>();

        public RealmDbContext(IDomainServiceLocator domainServiceLocator)
        {
            _domainServiceLocator = domainServiceLocator;
        }

        public async Task<IQueryable<T>> GetItemsAsync<T>() where T : RealmObject
        {
            Realms.Realm realm = await GetRealmAsync();
            return realm.All<T>();
        }

        public async Task<ITransaction?> StartTransactionAsync()
        {
            Realms.Realm realm = await GetRealmAsync();
            return realm.IsInTransaction ? null : new RealmTransaction(realm.BeginWrite());
        }

        public async Task<T> AddAsync<T>(T item, ITransaction transaction) where T : RealmObject
        {
            T? result = default;
            Realms.Realm realm = await GetRealmAsync();
            await this.RunInTransactionAsync(_ => { result = realm.Add(item); }, transaction);
            return result!;
        }

        public async Task UpdateItemAsync<T>(string id, Action<T> updateAction, ITransaction transaction) where T : RealmObject
        {
            Realms.Realm realm = await GetRealmAsync();
            await this.RunInTransactionAsync(_ =>
                                             {
                                                 T? item = null;
                                                 var newItem = _domainServiceLocator.Resolve<T>();
                                                 if(!string.IsNullOrWhiteSpace(id))
                                                 {
                                                     item = realm.Find<T>(id);
                                                 }

                                                 if(item != null)
                                                 {
                                                     updateAction(item);
                                                 }
                                                 else
                                                 {
                                                     updateAction(newItem);
                                                     realm.Add(newItem);
                                                 }
                                             }, transaction);
        }

        public async Task UpdateItemAsync<T>(string id, Func<T, Task> updateAction, ITransaction transaction) where T : RealmObject
        {
            Realms.Realm realm = await GetRealmAsync();
            await this.RunInTransactionAsync(async _ =>
                                             {
                                                 T? item = null;
                                                 var newItem = _domainServiceLocator.Resolve<T>();
                                                 if(!string.IsNullOrWhiteSpace(id))
                                                 {
                                                     item = realm.Find<T>(id);
                                                 }

                                                 if(item != null)
                                                 {
                                                     await updateAction(item);
                                                 }
                                                 else
                                                 {
                                                     await updateAction(newItem);
                                                     realm.Add(newItem);
                                                 }
                                             }, transaction);
        }

        public async Task RemoveAsync<T>(T item, ITransaction transaction) where T : RealmObject
        {
            Realms.Realm realm = await GetRealmAsync();
            await this.RunInTransactionAsync(_ => realm.Remove(item), transaction);
        }

        public async Task RemoveRangeAsync<T>(IQueryable<T> items, ITransaction transaction) where T : RealmObject
        {
            Realms.Realm realm = await GetRealmAsync();
            await this.RunInTransactionAsync(_ => realm.RemoveRange(items), transaction);
        }

        private async Task<Realms.Realm> GetRealmAsync()
        {
            Realms.Realm realm = await _domainServiceLocator.Resolve<IRealmFactory>().GetDefaultSyncedRealmAsync();
            if(!_activeRealms.Contains(realm))
            {
                _activeRealms.Add(realm);
            }
            return realm;
        }

        public void Dispose()
        {
            foreach(Realms.Realm activeRealm in _activeRealms)
            {
                activeRealm.Dispose();
            }
        }
    }
}