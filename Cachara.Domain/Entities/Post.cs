using Cachara.Domain.Entities.Common;
using Cachara.Domain.Interfaces;

namespace Cachara.Domain.Entities
{
    public class Post : EntityBase, ISoftDeletable
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public string Author { get; set; }

        public int Views { get; set; }

        public bool Deleted { get; set; }
    }
}