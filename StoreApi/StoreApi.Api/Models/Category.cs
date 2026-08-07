using System.ComponentModel.DataAnnotations;

namespace StoreApi.Api.Models;

public class Category
{
    public int Id { get; set;}
    [Required, MaxLength(50)]
    public required string Name { get; set; }
}