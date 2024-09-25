using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cachara.Data.Interfaces
{
    public interface IUnitOfWork
    {
        Task Discard();

        Task<int> Commit();
    }
}
