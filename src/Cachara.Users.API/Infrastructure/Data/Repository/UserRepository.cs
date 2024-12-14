using Cachara.Data.EF;
using Cachara.Data.Interfaces;
using Cachara.Shared.Infrastructure.Data.EF.Repositories;
using Cachara.Users.API.Domain.Entities;

namespace Cachara.Users.API.Infrastructure.Data.Repository;

public class UserRepository : EntityFrameworkRepository<CacharaUsersDbContext, User>, IUserRepository
{
    public UserRepository(CacharaUsersDbContext dbContext) : base(dbContext)
    {
    }
}