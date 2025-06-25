using AutoFixture;

namespace Cachara.Tests.Builders;

public abstract class BaseBuilder<TObject, TBuilder> where TBuilder : class
{
    protected IFixture Fixture { get; } = new Fixture();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected TObject Object;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public abstract TBuilder BuildDefault();

    public virtual TObject Create() =>
        Object;
}