using Minimarket.API;
using Minimarket.API.Services;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<Settings>(
    builder.Configuration.GetSection("Database")
);

builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<PaymentService>();
builder.Services.AddSingleton<CartService>();
builder.Services.AddSingleton<ReceiptService>();

// Register MongoDB client using configuration.
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<Settings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();
app.MapGet("/", async () => {
    return Results.Ok("Server is running...");
});

app.MapGet("/test-mongo", async (IMongoClient client) => {
    try {
        // The "ping" command is the best way to test connectivity
        var result = await client.GetDatabase("admin")
            .RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
        // Serialize BSON to JSON to avoid type-casting issues in the response.
        return Results.Ok(new { status = "OK", result = result.ToJson() });
    }
    catch (Exception ex) {
        return Results.Problem(ex.ToString());
    }
});

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
