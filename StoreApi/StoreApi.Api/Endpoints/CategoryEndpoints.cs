using Microsoft.EntityFrameworkCore;
using StoreApi.Api.Data;
using StoreApi.Api.ExceptionHandling;
using StoreApi.Api.Models;

namespace StoreApi.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/categories");

        group.MapGet("/", async (StoreContext db) =>
        {
            var categories = await db.Categories.AsNoTracking().ToListAsync();
            return Results.Ok(categories);
        });

        group.MapPost("/", async (Category category, StoreContext db) =>
        {
           db.Categories.Add(category);
           await db.SaveChangesAsync();
           return Results.Created($"/categories/{category.Id}", category); 
        })
        .RequireAuthorization();

        group.MapPut("/{id:int}", async (int id, Category updated, StoreContext db) =>
        {
            var category = await db.Categories.FindAsync(id);
            if (category is null) return Results.NotFound();

            category.Name = updated.Name;
            await db.SaveChangesAsync();
            return Results.Ok(category);
        })
        .RequireAuthorization();

        group.MapDelete("/{id:int}", async (int id, StoreContext db) =>
        {
           var category = await db.Categories.FindAsync(id);
           if (category is null) return Results.NotFound();

           db.Categories.Remove(category);
           await db.SaveChangesAsync();
           return Results.NoContent();
        }).RequireAuthorization();
    }
}