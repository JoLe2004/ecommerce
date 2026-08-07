using System.Net;
using System.Net.Http.Json;

namespace StoreApi.Tests;

public class ProductTests : AuthTestBase
{
    public ProductTests(CustomWebApplicationFactory factory) : base(factory) {}

    [Fact]
    public async Task GetProducts_ReturnsOk()
    {
        var response = await _client.GetAsync("/products");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithoutAuth_ReturnsUnauthorized()
    {
        var product = new { Name = "Shirt", Price = 15.99, CategoryName = "Apparel"};
        var response = await _client.PostAsJsonAsync("/products", product);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithAuth_ReturnsCreated()
    {
        await AuthenticateAsync();
        var category = new { Name = "Apparel" };
        var categoryResponse = await _client.PostAsJsonAsync("/categories/", category);
        categoryResponse.EnsureSuccessStatusCode();
        var createdCategory = await categoryResponse.Content.ReadFromJsonAsync<CategoryResponse>();
        var product = new { Name = "Shirt", Price = 15.99M, CategoryId = createdCategory!.Id};
        var response = await _client.PostAsJsonAsync("/products", product);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    private class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}