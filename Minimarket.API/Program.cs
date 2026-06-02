using Minimarket.API;
using Minimarket.API.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<Settings>(
    builder.Configuration.GetSection("Database"));

builder.Services.Configure<PricingSettings>(
    builder.Configuration.GetSection("Pricing"));
builder.Services.Configure<TaxSettings>(
    builder.Configuration.GetSection("Tax"));
builder.Services.Configure<PaymentFeeSettings>(
    builder.Configuration.GetSection("PaymentFees"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<Settings>>().Value;

    // Resolve DB Connection Password
    string? strCopy = settings.ConnectionString;
    int atIdx = strCopy!.IndexOf('@');
    string first = strCopy[..atIdx];
    string second = strCopy[atIdx..];

    StringBuilder connectionString = new();
    connectionString.Append(first);
    connectionString.Append(':');
    connectionString.Append(settings.Password);
    connectionString.Append(second);

    return new MongoClient(connectionString.ToString());
});

builder.Services.AddSingleton<CategoryService>();
builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<AuditLogService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<ReceiptService>();
builder.Services.AddSingleton<PricingRuleService>();
builder.Services.AddSingleton<MachineStateService>();
builder.Services.AddSingleton<CartService>();
builder.Services.AddSingleton<PaymentService>();
builder.Services.AddSingleton<DatabaseSeeder>();

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedPricingRulesAsync();
    await seeder.SeedMachineStatesAsync();
    await seeder.SeedUsersAsync();
}

app.MapGet("/", () => Results.Ok("Minimarket API is running."));

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
