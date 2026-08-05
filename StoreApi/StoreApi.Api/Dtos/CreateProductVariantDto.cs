namespace StoreApi.Api.Dtos;

public record CreateProductVariantDto(
    string Sku,
    string? Size,
    string? Color,
    int StockQuantity
);