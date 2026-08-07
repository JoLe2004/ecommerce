using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using StoreApi.Api.Data;

namespace StoreApi.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<StoreContext>) ||
                d.ServiceType == typeof(IDbContextOptionsConfiguration<StoreContext>))
                .ToList();

            foreach (var d in descriptors)
                services.Remove(d);

            services.AddDbContext<StoreContext>(options =>
                options.UseInMemoryDatabase("TestDb"));
        });
    }
}