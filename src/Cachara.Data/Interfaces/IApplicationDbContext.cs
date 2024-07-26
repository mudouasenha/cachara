using Cachara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cachara.Data.Interfaces
{
    public interface IApplicationContext : IUnitOfWork
    {
        DbSet<Post> Posts { get; }

        public IDbConnection Connection { get; }
    }
}
