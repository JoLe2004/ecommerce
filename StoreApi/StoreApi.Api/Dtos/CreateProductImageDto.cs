namespace StoreApi.Api.Dtos;

public record CreateProductImageDto(
    string Url,
    bool IsPrimary
);