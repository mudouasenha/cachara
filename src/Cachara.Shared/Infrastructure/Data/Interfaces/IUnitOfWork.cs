namespace Cachara.Shared.Infrastructure.Data.Interfaces
{
    public interface IUnitOfWork
    {
        Task Discard();

        Task<int> Commit();
    }
}
