namespace StoreApi.Api.Dtos;

public record ProductDto(
    int Id,
    string Name,
    decimal Price,
    string CategoryName
);