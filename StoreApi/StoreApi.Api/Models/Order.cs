using System.ComponentModel.DataAnnotations.Schema;

namespace StoreApi.Api.Models;

public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Cancelled
}

public class Order
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string ShippingName { get; set; }
    public required string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    [Column(TypeName = "decimal(7,2)")]
    public decimal TotalCost { get; set; }
    public bool IsRefunded { get; set; } = false;
    public DateTime? RefundedAt { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
}