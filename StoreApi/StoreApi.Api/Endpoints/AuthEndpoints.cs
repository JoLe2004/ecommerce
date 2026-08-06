using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreApi.Api.Auth;
using StoreApi.Api.Data;
using StoreApi.Api.Dtos;
using StoreApi.Api.Models;

namespace StoreApi.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/login", async (LoginDto dto, StoreContext db, JwtTokenGenerator tokenGen) =>
        {
            var admin = await db.Admins.FirstOrDefaultAsync(a => a.Email == dto.Email);
            if (admin is null) return Results.Unauthorized();

            var hasher = new PasswordHasher<Admin>();
            var result = hasher.VerifyHashedPassword(admin, admin.PasswordHash, dto.Password);
            if (result != PasswordVerificationResult.Success) return Results.Unauthorized();

            var token = tokenGen.GenerateToken(admin);
            return Results.Ok(new { token });
        });
    }
}