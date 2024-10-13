using Cachara.Content.API.Domain.Entities;
using Cachara.Data.Interfaces;
using Cachara.Data.Persistence.Repositories;
using Cachara.Domain.Entities;

namespace Cachara.Content.API.Infrastructure.Data.Repository;
    public class PostRepository : EntityFrameworkRepository<CacharaContentDbContext, Post>, IPostRepository
    {
        public PostRepository(CacharaContentDbContext dbContext) : base(dbContext)
        {
        }
    }
