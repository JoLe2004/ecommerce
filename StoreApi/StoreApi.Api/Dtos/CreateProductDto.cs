using System.ComponentModel.DataAnnotations;

namespace StoreApi.Api.Dtos;

public record CreateProductDto(
    [Required]
    string Name,
    [Required, Range(0.00, 200.00)]
    decimal Price,
    int CategoryId
);