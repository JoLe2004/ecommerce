using Microsoft.EntityFrameworkCore;
using StoreApi.Api.Data;
using StoreApi.Api.Dtos;
using StoreApi.Api.Models;

namespace StoreApi.Api.Endpoints;

public static class ProductImageEndpoints
{
    public static void MapProductImageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products/{productId:int}/images");

        group.MapGet("/", async (int productId, StoreContext db) =>
        {
            var images = await db.ProductImages
                .AsNoTracking()
                .Where(i => i.ProductId == productId)
                .Select(i => new ProductImageDto(i.Id, i.Url, i.IsPrimary))
                .ToListAsync();

            return Results.Ok(images);
        });

        group.MapPost("/", async (int productId, CreateProductImageDto dto, StoreContext db) =>
        {
            var product = await db.Products.FindAsync(productId);
            if (product is null) 
                return Results.Problem(detail: $"Product {productId} does not exist", statusCode: StatusCodes.Status400BadRequest);
            
            if (dto.IsPrimary)
            {
                await db.ProductImages
                    .Where(i => i.ProductId == productId && i.IsPrimary)
                    .ExecuteUpdateAsync(s => s.SetProperty(i => i.IsPrimary, false));
            }

            var productImage = new ProductImage
            {
                Url = dto.Url,
                IsPrimary = dto.IsPrimary,
                ProductId = productId,
                Product = product
            };

            db.ProductImages.Add(productImage);
            await db.SaveChangesAsync();

            var result = new ProductImageDto(productImage.Id, productImage.Url, productImage.IsPrimary);
            return Results.Created($"/products/{productId}/images/{productImage.Id}", result);
        });

        group.MapPut("/{id:int}", async (int productId, int id, CreateProductImageDto updated, StoreContext db) =>
        {
            var productImage = await db.ProductImages
                .FirstOrDefaultAsync(v => v.Id == id && v.ProductId == productId);
            if (productImage is null) return Results.NotFound();

            if (updated.IsPrimary)
            {
                await db.ProductImages
                    .Where(i => i.ProductId == productId && i.IsPrimary)
                    .ExecuteUpdateAsync(s => s.SetProperty(i => i.IsPrimary, false));
            }

            productImage.Url = updated.Url;
            productImage.IsPrimary = updated.IsPrimary;
            await db.SaveChangesAsync();
            var result = new ProductImageDto(productImage.Id, productImage.Url, productImage.IsPrimary);
            return Results.Ok(result);
        });

        group.MapDelete("/{id:int}", async (int productId, int id, StoreContext db) =>
        {
           var productImage = await db.ProductImages
                .FirstOrDefaultAsync(v => v.Id == id && v.ProductId == productId);
            if (productImage is null) return Results.NotFound();

           db.ProductImages.Remove(productImage);
           await db.SaveChangesAsync();
           return Results.NoContent();
        });
    }
}