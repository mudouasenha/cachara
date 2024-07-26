using Cachara.Data.EF;
using Cachara.Data.Interfaces;
using Cachara.Domain.Entities;

namespace Cachara.Data.Persistence.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly ApplicationContext _dbContext;
    private readonly IApplicationReadDbConnection _readDbConnection;
    private readonly IApplicationWriteDbConnection _writeDbConnection;

    public UserRepository(ApplicationContext dbContext, IApplicationReadDbConnection readDbConnection, IApplicationWriteDbConnection writeDbConnection) :
        base(dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _readDbConnection = readDbConnection ?? throw new ArgumentNullException(nameof(readDbConnection));
        _writeDbConnection = writeDbConnection ?? throw new ArgumentNullException(nameof(writeDbConnection));
    }
}