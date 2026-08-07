using Microsoft.EntityFrameworkCore;
using StoreApi.Api.Data;
using StoreApi.Api.Dtos;
using StoreApi.Api.Models;

namespace StoreApi.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders");

        group.MapGet("/", async (StoreContext db, int page = 1, int pageSize = 20) =>
        {
            var totalCount = await db.Orders.CountAsync();
            
            var orders = await db.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var orderDtos = orders.Select(order => new OrderDto(
                order.Id,
                order.Email,
                order.Status.ToString(),
                order.TotalCost,
                order.IsRefunded,
                order.RefundedAt,
                order.OrderItems.Select(oi => new OrderItemDto(
                    oi.ProductVariantId,
                    oi.Quantity,
                    oi.UnitPrice
                )).ToList()
            )).ToList();

            return Results.Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = orderDtos
            });
        }).RequireAuthorization();

        group.MapGet("/{id:int}", async (int id, StoreContext db) =>
        {
            var order = await db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
                return Results.NotFound();

            var orderDto = new OrderDto(
                order.Id,
                order.Email,
                order.Status.ToString(),
                order.TotalCost,
                order.IsRefunded,
                order.RefundedAt,
                order.OrderItems.Select(oi => new OrderItemDto(
                    oi.ProductVariantId,
                    oi.Quantity,
                    oi.UnitPrice
                )).ToList()
            );

            return Results.Ok(orderDto);
        });

        group.MapPost("/", async (CreateOrderDto dto, StoreContext db) =>
        {
            if (dto.Items.Count == 0)
                return Results.Problem(detail: $"Order must contain at least one item.", statusCode: StatusCodes.Status400BadRequest);

            var variantIds = dto.Items.Select(i => i.ProductVariantId).ToList();

            var variants = await db.ProductVariants
                .Include(v => v.Product)
                .Where(v => variantIds.Contains(v.Id))
                .ToListAsync();

            if (variants.Count != variantIds.Distinct().Count())
                return Results.Problem(detail: "One or more product variants do not exist.", statusCode: StatusCodes.Status400BadRequest);

            var orderItems = new List<OrderItem>();

            using var tx = await db.Database.BeginTransactionAsync();

            foreach (var item in dto.Items)
            {
                var variant = variants.First(v => v.Id == item.ProductVariantId);

                var rowsAffected = await db.ProductVariants
                    .Where(v => v.Id == variant.Id && v.StockQuantity >= item.Quantity)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(v => v.StockQuantity, v => v.StockQuantity - item.Quantity));

                if (rowsAffected == 0)
                {
                    await tx.RollbackAsync();
                    return Results.Problem(detail: $"Insufficient stock for variant {variant.Id}.", statusCode: 409);
                }

                orderItems.Add(new OrderItem
                {
                    ProductVariantId = variant.Id,
                    Quantity = item.Quantity,
                    UnitPrice = variant.Product.Price
                });
            }

            var totalCost = orderItems.Sum(oi => oi.Quantity * oi.UnitPrice);

            var order = new Order
            {
                Email = dto.Email,
                ShippingName = dto.ShippingName,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                Status = OrderStatus.Pending,
                TotalCost = totalCost,
                OrderItems = orderItems,
                IsRefunded = false,
                RefundedAt = null
            };

            db.Orders.Add(order);
            await db.SaveChangesAsync();
            await tx.CommitAsync();

            var orderDto = new OrderDto(
            order.Id,
            order.Email,
            order.Status.ToString(),
            order.TotalCost,
            order.IsRefunded,
            order.RefundedAt,
            order.OrderItems.Select(oi => new OrderItemDto(
                    oi.ProductVariantId,
                    oi.Quantity,
                    oi.UnitPrice
                )).ToList()
            );

            return Results.Created($"/orders/{order.Id}", orderDto);
        });

        group.MapPatch("/{id:int}/status", async (int id, UpdateOrderStatusDto dto, StoreContext db) =>
        {
            var order = await db.Orders.FindAsync(id);

            if (order is null)
                return Results.NotFound();

            order.Status = dto.Status;
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization();

        group.MapPatch("/{id:int}/cancel", async (int id, StoreContext db) =>
        {
            var order = await db.Orders.FindAsync(id);

            if (order is null)
                return Results.NotFound();

            if (order.Status == OrderStatus.Cancelled)
                return Results.Problem("Order is already cancelled.", statusCode: 400);

            if (order.Status == OrderStatus.Paid)
            {
                // TODO Phase 7: trigger Stripe refund here
                // IsRefunded stays false until that succeeds
            }

            order.Status = OrderStatus.Cancelled;
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization();
    }
}