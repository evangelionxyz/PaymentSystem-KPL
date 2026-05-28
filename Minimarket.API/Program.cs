using Minimarket.API;
using Minimarket.API.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// ── Database settings ──────────────────────────────────────────────────────
builder.Services.Configure<Settings>(
    builder.Configuration.GetSection("Database"));

// ── Runtime configuration (IOptions<T>) — Task 4.5 ───────────────────────
builder.Services.Configure<PricingSettings>(
    builder.Configuration.GetSection("Pricing"));
builder.Services.Configure<TaxSettings>(
    builder.Configuration.GetSection("Tax"));
builder.Services.Configure<PaymentFeeSettings>(
    builder.Configuration.GetSection("PaymentFees"));

// ── Singleton MongoDB client ───────────────────────────────────────────────
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<Settings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// ── Core services — Task 6.6 ──────────────────────────────────────────────
builder.Services.AddSingleton<CategoryService>();
builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<ReceiptService>();
builder.Services.AddSingleton<PricingRuleService>();
builder.Services.AddSingleton<MachineStateService>();
builder.Services.AddSingleton<CartService>();
builder.Services.AddSingleton<PaymentService>();
builder.Services.AddSingleton<DatabaseSeeder>();

// ── ASP.NET Core ───────────────────────────────────────────────────────────
builder.Services.AddControllers();

var app = builder.Build();

// ── Seed database on startup — Task 5.4 ──────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedPricingRulesAsync();
    await seeder.SeedMachineStatesAsync();
}

// ── Routes ────────────────────────────────────────────────────────────────
app.MapGet("/", () => Results.Ok("Minimarket API is running."));

// ── Pipeline ──────────────────────────────────────────────────────────────
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
