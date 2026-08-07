namespace StoreApi.Api.Dtos;

public record OrderItemDto
(
    int ProductVariantId,
    int Quantity,
    decimal UnitPrice
);