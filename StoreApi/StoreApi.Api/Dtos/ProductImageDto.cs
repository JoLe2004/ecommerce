namespace StoreApi.Api.Dtos;

public record ProductImageDto(
    int Id,
    string Url,
    bool IsPrimary
);