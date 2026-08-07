using System.ComponentModel.DataAnnotations.Schema;

namespace StoreApi.Api.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
    public required int Quantity { get; set; }
    [Column(TypeName = "decimal(5,2)")]
    public decimal UnitPrice { get; set; }
}