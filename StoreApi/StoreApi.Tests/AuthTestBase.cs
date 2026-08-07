

using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace StoreApi.Tests;

public class AuthTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly HttpClient _client;

    public AuthTestBase(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    protected async Task AuthenticateAsync()
    {
        var login = new { Email = "admin@store.com", Password = "admin123"};
        var response = await _client.PostAsJsonAsync("/auth/login", login);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", result!.Token);
    }

    private class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}