
using Microsoft.EntityFrameworkCore;
using StoreApi.Api.Data;
using StoreApi.Api.Dtos;
using StoreApi.Api.Models;

namespace StoreApi.Api.Endpoints;

public static class ProductVariantEndpoints
{
    public static void MapProductVariantEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products/{productId:int}/variants");

        group.MapGet("/", async (int productId, StoreContext db) =>
        {
            var variants = await db.ProductVariants
                .AsNoTracking()
                .Where(v => v.ProductId == productId)
                .Select(v => new ProductVariantDto(v.Id, v.Sku, v.Size, v.Color, v.StockQuantity))
                .ToListAsync();

            return Results.Ok(variants);
        });

        group.MapPost("/", async (int productId, CreateProductVariantDto dto, StoreContext db) =>
        {
            var product = await db.Products.FindAsync(productId);
            if (product is null) 
                return Results.Problem(detail: $"Product {productId} does not exist", statusCode: StatusCodes.Status400BadRequest);
            
            var productVariant = new ProductVariant
            {
                Sku = dto.Sku,
                Size = dto.Size,
                Color = dto.Color,
                StockQuantity = dto.StockQuantity,
                ProductId = productId,
                Product = product
            };

            db.ProductVariants.Add(productVariant);
            await db.SaveChangesAsync();

            var result = new ProductVariantDto(productVariant.Id, productVariant.Sku, productVariant.Size, productVariant.Color, productVariant.StockQuantity);
            return Results.Created($"/products/{productId}/variants/{productVariant.Id}", result);
        }).RequireAuthorization();

        group.MapPut("/{id:int}", async (int productId, int id, CreateProductVariantDto updated, StoreContext db) =>
        {
            var productVariant = await db.ProductVariants
                .FirstOrDefaultAsync(v => v.Id == id && v.ProductId == productId);
            if (productVariant is null) return Results.NotFound();


            productVariant.Sku = updated.Sku;
            productVariant.Size = updated.Size;
            productVariant.Color = updated.Color;
            productVariant.StockQuantity = updated.StockQuantity;
            await db.SaveChangesAsync();
            var result = new ProductVariantDto(productVariant.Id, productVariant.Sku, productVariant.Size, productVariant.Color, productVariant.StockQuantity);
            return Results.Ok(result);
        }).RequireAuthorization();

        group.MapDelete("/{id:int}", async (int productId, int id, StoreContext db) =>
        {
           var productVariant = await db.ProductVariants
                .FirstOrDefaultAsync(v => v.Id == id && v.ProductId == productId);
            if (productVariant is null) return Results.NotFound();

           db.ProductVariants.Remove(productVariant);
           await db.SaveChangesAsync();
           return Results.NoContent();
        }).RequireAuthorization();
    }
}