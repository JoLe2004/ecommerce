namespace StoreApi.Api.Models;

public class Product
{
    public int Id { get; set;}
    public required string Name { get; set; }
    public decimal Price {get; set;}
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public List<ProductVariant> Variants { get; set; } = new();
    public List<ProductImage> Images { get; set; } = new();
}