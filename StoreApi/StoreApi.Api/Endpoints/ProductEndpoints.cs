using Microsoft.EntityFrameworkCore;
using StoreApi.Api.Data;
using StoreApi.Api.Dtos;
using StoreApi.Api.Models;

namespace StoreApi.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products");

        group.MapGet("/", async (StoreContext db) =>
        {
            var products = await db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Category.Name))
                .ToListAsync();
            
            return Results.Ok(products);
        });

        group.MapPost("/", async (CreateProductDto dto, StoreContext db) =>
        {
            var category = await db.Categories.FindAsync(dto.CategoryId);
            if (category is null) 
                return Results.Problem(detail: $"Category {dto.CategoryId} does not exist", statusCode: StatusCodes.Status400BadRequest);

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                Category = category
            };

            db.Products.Add(product);
            await db.SaveChangesAsync();

            var result = new ProductDto(product.Id, product.Name, product.Price, category.Name);
            return Results.Created($"/products/{product.Id}", result);
        });

        group.MapPut("/{id:int}", async (int id, CreateProductDto updated, StoreContext db) =>
        {
            var product = await db.Products.FindAsync(id);
            if (product is null) return Results.NotFound();

            var category = await db.Categories.FindAsync(updated.CategoryId);
            if (category is null)
                return Results.Problem(detail: $"Category {updated.CategoryId} does not exist", statusCode: StatusCodes.Status400BadRequest);

            product.Name = updated.Name;
            product.Price = updated.Price;
            product.CategoryId = updated.CategoryId;
            product.Category = category;
            await db.SaveChangesAsync();
            var result = new ProductDto(product.Id, product.Name, product.Price, category.Name);
            return Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int id, StoreContext db) =>
        {
           var product = await db.Products.FindAsync(id);
           if (product is null) return Results.NotFound();

           db.Products.Remove(product);
           await db.SaveChangesAsync();
           return Results.NoContent();
        });

    }
}