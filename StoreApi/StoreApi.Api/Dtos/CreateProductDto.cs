namespace StoreApi.Api.Dtos;

public record CreateProductDto(
    string Name,
    decimal Price,
    int CategoryId
);