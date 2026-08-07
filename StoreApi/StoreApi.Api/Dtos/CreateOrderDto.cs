namespace StoreApi.Api.Dtos;

public record CreateOrderDto
(
    string Email,
    string ShippingName,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string PostalCode,
    string Country,
    List<CreateOrderItemDto> Items
);