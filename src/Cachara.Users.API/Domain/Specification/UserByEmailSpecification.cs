using Cachara.Shared.Domain.Specification;
using Cachara.Users.API.Domain.Entities;

namespace Cachara.Users.API.Domain.Specification;

public class UserByEmailSpecification : BaseSpecification<User>
{
    private string _email;

    public UserByEmailSpecification(string email)
    {
        _email = email;
    }
    public bool IsSatisfied(User u)
    {
        return u.Email == _email;
    }
}