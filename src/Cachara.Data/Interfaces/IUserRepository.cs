using Cachara.Domain.Entities;

namespace Cachara.Data.Interfaces;

public interface IUserRepository : IRepository<User>, IReadRepository<User>
{
    
}