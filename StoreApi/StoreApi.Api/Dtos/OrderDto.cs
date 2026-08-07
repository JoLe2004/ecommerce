namespace StoreApi.Api.Dtos;

public record OrderDto
(
    int Id,
    string Email,
    string Status,
    decimal TotalCost,
    bool IsRefunded,
    DateTime? RefundedAt,
    List<OrderItemDto> Items
);