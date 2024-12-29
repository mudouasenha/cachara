namespace Cachara.Content.API.Infrastructure.Clients;

public class GitHubService
{
    private readonly IHttpClientFactory _factory;


    public GitHubService()
    {
        
    }

    public Task Test()
    {
        var client = _factory.CreateClient();

        return Task.CompletedTask;
    }
}