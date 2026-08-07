namespace StoreApi.Api.Dtos;
public record CreateOrderItemDto
(
    int ProductVariantId,
    int Quantity
);