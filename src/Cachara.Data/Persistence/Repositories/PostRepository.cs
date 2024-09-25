using Cachara.Data.EF;
using Cachara.Data.Interfaces;
using Cachara.Domain.Entities;

namespace Cachara.Data.Persistence.Repositories;
    public class PostRepository : EntityFrameworkRepository<CacharaSocialDbContext, Post>, IPostRepository
    {
        public PostRepository(CacharaSocialDbContext dbContext) : base(dbContext)
        {
        }
    }
