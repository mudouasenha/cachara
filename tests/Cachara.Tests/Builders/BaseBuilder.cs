using AutoFixture;

namespace Cachara.Tests.Builders;

public abstract class BaseBuilder<TObject, TBuilder> where TBuilder : class
{
    protected IFixture Fixture { get; } = new Fixture();
    protected TObject Object;

    public abstract TBuilder BuildDefault();

    public virtual TObject Create() =>
        Object;
}