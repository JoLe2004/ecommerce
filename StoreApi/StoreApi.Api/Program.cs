using Microsoft.EntityFrameworkCore;
using StoreApi.Api.Data;
using StoreApi.Api.Endpoints;
using StoreApi.Api.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StoreContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("StoreContext")));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/", () => "Hello World");
app.MapCategoryEndpoints();
app.MapProductEndpoints();
app.MapProductVariantEndpoints();

app.Run();