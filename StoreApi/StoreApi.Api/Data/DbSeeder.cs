using Microsoft.AspNetCore.Identity;
using StoreApi.Api.Data;
using StoreApi.Api.Models;

public static class DbSeeder
{
    public static void SeedAdmin(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StoreContext>();

        if (db.Admins.Any()) return;

        // In actual prod, use a secrets manager instead of config!
        var email = config["SeedAdmin:Email"];
        var password = config["SeedAdmin:Password"];
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            throw new InvalidOperationException("SeedAdmin config missing.");

        var hasher = new PasswordHasher<Admin>();
        var admin = new Admin { Email = email, PasswordHash = "" };
        admin.PasswordHash = hasher.HashPassword(admin, password);

        db.Admins.Add(admin);
        db.SaveChanges();
    }
}