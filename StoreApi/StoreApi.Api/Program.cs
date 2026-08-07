using Microsoft.EntityFrameworkCore;
using StoreApi.Api.Auth;
using StoreApi.Api.Data;
using StoreApi.Api.Endpoints;
using StoreApi.Api.ExceptionHandling;
using StoreApi.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StoreContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("StoreContext")));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddScoped<JwtTokenGenerator>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World");
app.MapAuthEndpoints();
app.MapCategoryEndpoints();
app.MapOrderEndpoints();
app.MapProductEndpoints();
app.MapProductImageEndpoints();
app.MapProductVariantEndpoints();

DbSeeder.SeedAdmin(app.Services, app.Configuration);
app.Run();