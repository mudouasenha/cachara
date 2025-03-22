using AutoFixture;
using AutoMapper;
using Cachara.Users.API.Services.Mappings;

namespace Cachara.Tests.Unit.Mappings;

public class SocialProfileTests
{
    [Trait("Category", "Unit")]
    [Trait("Feature", "Mapper")]
    [Trait("Profile", "Social")]
    [Fact(DisplayName = "Profile Should Have Valid Configuration")]
    public void Profile_ShouldHaveValidConfiguration()
    {
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(new UsersMappings()));
        mapperConfiguration.AssertConfigurationIsValid();
        mapperConfiguration.CreateMapper();
    }
}