using Cachara.Content.API.Domain.Entities;
using Cachara.Domain.Abstractions.Specification;

namespace Cachara.Content.API.Domain.Specification;

public class PostUserNameSpecification : BaseSpecification<Post>
{
    private string _userName;

    public PostUserNameSpecification(string userName)
    {
        _userName = userName;
    }
    public bool IsSatisfied(Post t)
    {
        return t.AuthorId == _userName;
    }
}