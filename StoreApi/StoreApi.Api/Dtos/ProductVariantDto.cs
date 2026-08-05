namespace StoreApi.Api.Dtos;

public record ProductVariantDto(
    int Id,
    string Sku,
    string? Size,
    string? Color,
    int StockQuantity
);