using Cachara.Domain.Abstractions.Specification;
using Cachara.Domain.Entities;

namespace Cachara.Domain.Specification;

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