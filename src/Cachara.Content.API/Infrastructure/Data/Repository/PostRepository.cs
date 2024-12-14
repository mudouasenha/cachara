using Cachara.Content.API.Domain.Entities;
using Cachara.Data.Interfaces;
using Cachara.Shared.Infrastructure.Data.EF.Repositories;

namespace Cachara.Content.API.Infrastructure.Data.Repository;
    public class PostRepository : EntityFrameworkRepository<CacharaContentDbContext, Post>, IPostRepository
    {
        public PostRepository(CacharaContentDbContext dbContext) : base(dbContext)
        {
        }
    }
