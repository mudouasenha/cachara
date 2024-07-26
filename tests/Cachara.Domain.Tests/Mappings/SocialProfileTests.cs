using AutoFixture;
using AutoMapper;
using Cachara.Domain.Mappings;

namespace Cachara.Domain.Tests.Mappings;

public class SocialProfileTests
{
    private IMapper _sut;
    private readonly Fixture _fixture;
    
    public SocialProfileTests()
    {
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(new SocialMappings()));
        mapperConfiguration.AssertConfigurationIsValid();
        _sut = mapperConfiguration.CreateMapper();
    }

    [Fact]
    public void Profile_ShouldHaveValidConfiguration()
    {
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(new SocialMappings()));
        mapperConfiguration.AssertConfigurationIsValid();
        _sut = mapperConfiguration.CreateMapper();
    }
}