using Cachara.Data.EF;
using Cachara.Data.Interfaces;
using Cachara.Domain.Entities;

namespace Cachara.Data.Persistence.Repositories;

public class UserRepository : EntityFrameworkRepository<CacharaSocialDbContext, User>, IUserRepository
{
    public UserRepository(CacharaSocialDbContext dbContext) : base(dbContext)
    {
    }
}