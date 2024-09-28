using Cachara.Domain.Entities.Common;
using Cachara.Domain.Interfaces;
using FluentValidation.Results;

namespace Cachara.Domain.Entities
{
    public class Post : IEntity<string>, IModifiable, IVersable, ISoftDeletable, IValidatable
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public string Body { get; set; }

        public string AuthorId { get; set; }
        
        public User Author { get; set; }

        public bool Deleted { get; set; }

        public byte[] Version { get; set; }
        
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
        public ValidationResult Validate()
        {
            throw new NotImplementedException();
        }

        public void ValidateAndThrow()
        {
            throw new NotImplementedException();
        }
    }
}