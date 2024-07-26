using AutoMapper;
using Cachara.Core.Models;

namespace Cachara.Domain.Mappings;

public class SocialMappings : Profile
{
    public SocialMappings()
    {
        CreateMap<Entities.Post, PostBase>();
        
    }
}