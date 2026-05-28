using Minimarket.API;
using Minimarket.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<Settings>(
    builder.Configuration.GetSection("CustomerDatabase")
);

builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<PaymentService>();
builder.Services.AddSingleton<CartService>();
builder.Services.AddSingleton<ReceiptService>();

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
