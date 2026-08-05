namespace StoreApi.Api.Models;

public class ProductVariant
{
    public int Id { get; set;}
    public required string Sku { get; set; }
    public string? Size {get; set;}
    public string? Color { get; set; }
    public int StockQuantity { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}