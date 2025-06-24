using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Services.Mappings;
using FluentAssertions;
using Mapster;

namespace Cachara.Tests.Unit.Mappings;

public class UserMappingProfileTests
{
    [Trait("Category", "Unit")]
    [Trait("Feature", "Mapper")]
    [Trait("Profile", "Social")]
    [Fact(DisplayName = "Profile Should Have Valid Configuration")]
    public void Profile_ShouldHaveValidConfiguration()
    {
        // Arrange
        var config = new TypeAdapterConfig();
        UsersMappings.Configure();
        config.Compile();
    
        // Act
        var testUser = new User { Id = Guid.NewGuid().ToString(), UserName = "Test" };
        var result = testUser.Adapt<Cachara.Users.API.Services.Models.User>();
    
        // Assert
        result.Should().NotBeNull();
    }
}