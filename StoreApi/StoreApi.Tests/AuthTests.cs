using System.Net;
using System.Net.Http.Json;

namespace StoreApi.Tests;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LoginWithValidCredentials_ReturnsToken()
    {
        var login = new { Email = "admin@store.com", Password = "admin123"};

        var response = await _client.PostAsJsonAsync("/auth/login", login);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task LoginWithInvalidPassword_ReturnsUnauthorized()
    {
        var login = new { Email = "admin@store.com", Password = "wrong"};

        var response = await _client.PostAsJsonAsync("/auth/login", login);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task LoginWithInvalidEmail_ReturnsUnauthorized()
    {
        var login = new { Email = "admin@s.com", Password = "admin123"};

        var response = await _client.PostAsJsonAsync("/auth/login", login);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

}