namespace Xamflix.Domain.Data
{
    public interface IDomainServiceLocator
    {
        T Resolve<T>();
    }
}