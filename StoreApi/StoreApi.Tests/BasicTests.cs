using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace StoreApi.Tests;

public class BasicTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BasicTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Root_ReturnsHelloWorld()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal("Hello World", body);
    }
}