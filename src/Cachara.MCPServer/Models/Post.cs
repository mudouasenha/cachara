namespace Cachara.Content.MCPServer.Models;

public record Post(
    string Title,
    string Body,
    string AuthorId,
    string Id,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
