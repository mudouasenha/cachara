using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Cachara.Content.MCPServer.Models;

namespace Cachara.Content.MCPServer.Clients;

// TODO: fix httpCLient
public class PostsClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public PostsClient(HttpClient client)
    {
        _httpClient = client;
        _jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }

    public async Task<List<Post>> GetPostsAsync(string userId)
    {
        var response = await _httpClient.GetAsync($"https://graph.microsoft.com/beta/users/{userId}");

        if (response.StatusCode == HttpStatusCode.NoContent)
            return new List<Post>();

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Post>>(_jsonSerializerOptions);
    }
}
