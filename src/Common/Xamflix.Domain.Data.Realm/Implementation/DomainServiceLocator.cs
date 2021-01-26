using System;
using Microsoft.Extensions.DependencyInjection;

namespace Xamflix.Domain.Data.Realm.Implementation
{
    public class DomainServiceLocator : IDomainServiceLocator
    {
        private readonly IServiceProvider _services;

        public DomainServiceLocator(IServiceProvider services)
        {
            _services = services;
        }
        
        public T Resolve<T>()
        {
            return _services.GetRequiredService<T>();
        }
    }
}