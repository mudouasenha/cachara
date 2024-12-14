using Cachara.Shared.Domain.Entities.Abstractions;

namespace Cachara.Shared.Domain;

public static class IEntityExtensions
{
    public static string GenerateId(this IEntity<string> entity, bool force = false)
    {
        //It's worth noting that ITU REC X.667 itu.int/rec/T-REC-X.667-201210-I/en which covers UUIDS states in section 6.5.4
        //"Software generating the hexadecimal representation of a UUID shall not use upper case letters.
        //NOTE – It is recommended that the hexadecimal representation used in all human-readable formats be restricted to lower-case letters.
        //Software processing this representation is, however, required to accept both upper and lower case letters as specified in 6.5.2."
        //So when generating them, it's advisable to use only lower case letters.

        if (string.IsNullOrEmpty(entity.Id) || force)
        {
            entity.Id = Guid.NewGuid().ToString("D").ToLowerInvariant();
        }

        return entity.Id;
    }

    public static void UpdateCreatedAt(this IModifiable entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
    }

    public static void UpdateUpdateAt(this IModifiable entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
    }
}