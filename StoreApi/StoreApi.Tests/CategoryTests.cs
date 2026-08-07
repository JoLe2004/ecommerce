using System.Net;
using System.Net.Http.Json;

namespace StoreApi.Tests;

public class CategoryTests : AuthTestBase
{
    public CategoryTests(CustomWebApplicationFactory factory) : base(factory) {}

    [Fact]
    public async Task GetCategories_ReturnsOk()
    {
        var response = await _client.GetAsync("/categories");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_WithoutAuth_ReturnsUnauthorized()
    {
        var category = new { Name = "Test Category"};
        var response = await _client.PostAsJsonAsync("/categories", category);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_WithAuth_ReturnsCreated()
    {
        await AuthenticateAsync();
        var category = new { Name = "Test Category"};
        var response = await _client.PostAsJsonAsync("/categories", category);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_EmptyName_ReturnsBadRequest()
    {
        await AuthenticateAsync();
        var category = new { Name = ""};
        var response = await _client.PostAsJsonAsync("/categories", category);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_LongName_ReturnsBadRequest()
    {
        await AuthenticateAsync();
        var category = new { Name = "Reallyreallyreallyreallyreallyreallyreallyreallyreallyreallyreallyreallyreallyname"};
        var response = await _client.PostAsJsonAsync("/categories", category);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_WithoutAuth_ReturnsUnauthorized()
    {
        var updated = new { Name = "Updated Name"};
        var response = await _client.PutAsJsonAsync("/categories/1", updated);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_WithAuth_ReturnsOk()
    {
        await AuthenticateAsync();
        var create = new { Name = "Test Category"};
        var createResponse = await _client.PostAsJsonAsync("/categories", create);
        var created = await createResponse.Content.ReadFromJsonAsync<CategoryResponse>();
        var updated = new { Name = "Updated Name"};
        var response = await _client.PutAsJsonAsync($"/categories/{created!.Id}", updated);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

        [Fact]
    public async Task DeleteCategory_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await _client.DeleteAsync("/categories/1");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_WithAuth_ReturnsOk()
    {
        await AuthenticateAsync();
        var create = new { Name = "Test Category"};
        var createResponse = await _client.PostAsJsonAsync("/categories", create);
        var created = await createResponse.Content.ReadFromJsonAsync<CategoryResponse>();
        var response = await _client.DeleteAsync($"/categories/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}