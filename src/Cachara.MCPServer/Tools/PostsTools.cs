using System.ComponentModel;
using System.Text.Json;
using Cachara.Content.MCPServer.Clients;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace Cachara.Content.MCPServer.Tools;

[McpServerToolType]
public static class PostsTools
{
    [McpServerTool, Description("Busca os posts de determinado usuário, utilizando como filtro o id do usuário")]
    public static async Task<string> GetPosts(PostsClient postsClient,
        [Description("Filtro obrigatório pelo id do usuário")] string userId,
        ILogger? logger = null)
    {
        try
        {
            var posts = await postsClient.GetPostsAsync(userId);

            if (posts.Count == 0)
            {
                logger?.LogWarning("No posts found for user {UserId}", userId);
                return "Books not found";
            }


            logger?.LogInformation("Fetched {Count} posts for user {UserId}", posts.Count, userId);
            return JsonSerializer.Serialize(posts);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error fetching posts for user {UserId}", userId);
            return $"Error fetching posts for user {userId}, message: {ex.Message}";
        }

    }
}
