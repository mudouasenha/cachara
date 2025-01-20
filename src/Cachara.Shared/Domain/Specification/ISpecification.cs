using System.Linq.Expressions;

namespace Cachara.Shared.Domain.Specification;

public interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);
}

public abstract class BaseSpecification<T> : ISpecification<T> where T : class
{
    protected Expression<Func<T, bool>> _expression;

    public bool IsSatisfiedBy(T entity)
    {
        return _expression.Compile()(entity);
    }

    public Expression<Func<T, bool>> ToExpression()
    {
        return _expression;
    }

    public Expression<Func<T, bool>> And(BaseSpecification<T> otherSpec)
    {
        var invokedExpr = Expression.Invoke(otherSpec.ToExpression(), _expression.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.AndAlso(_expression.Body, invokedExpr), _expression.Parameters);
    }
}
