using AutoFixture;
using AutoMapper;
using Cachara.Domain.Mappings;

namespace Cachara.Tests.Unit.Mappings;

public class SocialProfileTests
{
    private IMapper _sut;
    private readonly Fixture _fixture;

    [Trait("Category", "Unit")]
    [Trait("Feature", "Mapper")]
    [Trait("Profile", "Social")]
    [Fact(DisplayName = "Profile Should Have Valid Configuration")]
    public void Profile_ShouldHaveValidConfiguration()
    {
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(new SocialMappings()));
        mapperConfiguration.AssertConfigurationIsValid();
        _sut = mapperConfiguration.CreateMapper();
    }
}