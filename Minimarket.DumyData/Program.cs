using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Minimarket.Core.Models;

Console.WriteLine("=== Minimarket Database Seeder ===");

const string connectionString = "mongodb://admin:YIDzxt25087@node76037-evangelion.user.cloudjkt02.com:11339/Minimarket?authSource=admin";
const string dbName = "Minimarket";

Console.WriteLine("Connecting to MongoDB...");
var client = new MongoClient(connectionString);
var db = client.GetDatabase(dbName);

// Get Collections
var categoryColl = db.GetCollection<Category>("categories");
var customerColl = db.GetCollection<Customer>("customers");
var productColl = db.GetCollection<Product>("products");
var cartColl = db.GetCollection<Cart>("carts");
var pricingRulesColl = db.GetCollection<PricingRule>("pricingRules");
var paymentColl = db.GetCollection<Payment>("payments");
var receiptColl = db.GetCollection<Receipt>("receipts");

// 1. Seed Categories (15+ items)
Console.WriteLine("Seeding Categories...");
var categoriesToSeed = new List<Category>
{
    new() { Name = "Snack", Description = "Delicious snacks and chips" },
    new() { Name = "Beverage", Description = "Cold and hot drinks" },
    new() { Name = "Fresh", Description = "Fresh vegetables and fruits" },
    new() { Name = "Household", Description = "Cleaning supplies and tools" },
    new() { Name = "Dairy", Description = "Milk, cheese, and butter" },
    new() { Name = "Bakery", Description = "Bread, buns, and pastries" },
    new() { Name = "Frozen", Description = "Ice cream and frozen meals" },
    new() { Name = "Meat", Description = "Chicken, beef, and fish" },
    new() { Name = "Produce", Description = "Fresh farm produce" },
    new() { Name = "PersonalCare", Description = "Shampoo, soap, and hygiene products" },
    new() { Name = "Baby", Description = "Diapers and baby food" },
    new() { Name = "Pet", Description = "Pet food and toys" },
    new() { Name = "Paper", Description = "Tissues, paper towels, and paper plates" },
    new() { Name = "Pharmacy", Description = "Over-the-counter medicine and vitamins" },
    new() { Name = "Candy", Description = "Sweets, chocolates, and gums" },
    new() { Name = "Books", Description = "Notebooks, books, and magazines" }
};

var dbCategories = new List<Category>();
foreach (var cat in categoriesToSeed)
{
    var existing = await categoryColl.Find(c => c.Name.ToLower() == cat.Name.ToLower()).FirstOrDefaultAsync();
    if (existing == null)
    {
        await categoryColl.InsertOneAsync(cat);
        dbCategories.Add(cat);
        Console.WriteLine($"-> Created Category: '{cat.Name}'");
    }
    else
    {
        dbCategories.Add(existing);
    }
}
Console.WriteLine($"Total Categories in DB: {await categoryColl.CountDocumentsAsync(FilterDefinition<Category>.Empty)}");

// 2. Seed Customers (15+ items, with unique phones)
Console.WriteLine("\nSeeding Customers...");
var customersToSeed = new List<Customer>
{
    new() { FirstName = "John", LastName = "Doe", Phone = "+628111111101" },
    new() { FirstName = "Jane", LastName = "Smith", Phone = "+628111111102" },
    new() { FirstName = "Alice", LastName = "Johnson", Phone = "+628111111103" },
    new() { FirstName = "Bob", LastName = "Williams", Phone = "+628111111104" },
    new() { FirstName = "Charlie", LastName = "Brown", Phone = "+628111111105" },
    new() { FirstName = "David", LastName = "Jones", Phone = "+628111111106" },
    new() { FirstName = "Eve", LastName = "Garcia", Phone = "+628111111107" },
    new() { FirstName = "Frank", LastName = "Miller", Phone = "+628111111108" },
    new() { FirstName = "Grace", LastName = "Davis", Phone = "+628111111109" },
    new() { FirstName = "Hank", LastName = "Rodriguez", Phone = "+628111111110" },
    new() { FirstName = "Ivy", LastName = "Martinez", Phone = "+628111111111" },
    new() { FirstName = "Jack", LastName = "Hernandez", Phone = "+628111111112" },
    new() { FirstName = "Karen", LastName = "Lopez", Phone = "+628111111113" },
    new() { FirstName = "Leo", LastName = "Gonzalez", Phone = "+628111111114" },
    new() { FirstName = "Mia", LastName = "Wilson", Phone = "+628111111115" },
    new() { FirstName = "Nora", LastName = "Anderson", Phone = "+628111111116" }
};

var dbCustomers = new List<Customer>();
foreach (var cust in customersToSeed)
{
    var existing = await customerColl.Find(c => c.Phone == cust.Phone).FirstOrDefaultAsync();
    if (existing == null)
    {
        await customerColl.InsertOneAsync(cust);
        dbCustomers.Add(cust);
        Console.WriteLine($"-> Created Customer: '{cust.FirstName} {cust.LastName}' ({cust.Phone})");
    }
    else
    {
        dbCustomers.Add(existing);
    }
}
Console.WriteLine($"Total Customers in DB: {await customerColl.CountDocumentsAsync(FilterDefinition<Customer>.Empty)}");

// 3. Seed Products (15+ items, with unique names)
Console.WriteLine("\nSeeding Products...");
var productsToSeed = new[]
{
    (Name: "Oreo Pack", Desc: "Chocolate sandwich cookies", Price: 12000m, CatName: "Snack"),
    (Name: "Coca-Cola Can", Desc: "Carbonated soft drink", Price: 7500m, CatName: "Beverage"),
    (Name: "Red Apples", Desc: "Fresh organic apples", Price: 35000m, CatName: "Fresh"),
    (Name: "Floor Cleaner", Desc: "Lavender scent floor cleaner", Price: 18000m, CatName: "Household"),
    (Name: "Fresh Milk 1L", Desc: "Pasteurized whole milk", Price: 24000m, CatName: "Dairy"),
    (Name: "Wholewheat Bread", Desc: "Freshly baked wholewheat bread", Price: 15000m, CatName: "Bakery"),
    (Name: "Frozen Chicken Nuggets", Desc: "Crispy chicken nuggets 500g", Price: 45000m, CatName: "Frozen"),
    (Name: "Beef Sirloin 200g", Desc: "Fresh beef sirloin steak", Price: 85000m, CatName: "Meat"),
    (Name: "Cavendish Banana", Desc: "Sweet yellow banana 1kg", Price: 22000m, CatName: "Produce"),
    (Name: "Herbal Shampoo", Desc: "Anti-dandruff shampoo", Price: 32000m, CatName: "PersonalCare"),
    (Name: "Baby Wipes 80s", Desc: "Fragrance-free baby wipes", Price: 16500m, CatName: "Baby"),
    (Name: "Premium Dog Food 1kg", Desc: "Dry food for adult dogs", Price: 55000m, CatName: "Pet"),
    (Name: "Facial Tissue 200s", Desc: "Soft facial tissue", Price: 9500m, CatName: "Paper"),
    (Name: "Paracetamol 500mg", Desc: "Fever and pain relief", Price: 5000m, CatName: "Pharmacy"),
    (Name: "Milk Chocolate Bar", Desc: "Smooth milk chocolate 100g", Price: 20000m, CatName: "Candy"),
    (Name: "Spiral Notebook", Desc: "A5 spiral notebook 80 pages", Price: 11000m, CatName: "Books")
};

var dbProducts = new List<Product>();
foreach (var prodData in productsToSeed)
{
    var cat = dbCategories.FirstOrDefault(c => c.Name.Equals(prodData.CatName, StringComparison.OrdinalIgnoreCase));
    var product = new Product
    {
        Name = prodData.Name,
        Description = prodData.Desc,
        Price = prodData.Price,
        CategoryId = cat?.ID,
        CategoryName = cat?.Name
    };

    var existing = await productColl.Find(p => p.Name.ToLower() == product.Name.ToLower()).FirstOrDefaultAsync();
    if (existing == null)
    {
        await productColl.InsertOneAsync(product);
        dbProducts.Add(product);
        Console.WriteLine($"-> Created Product: '{product.Name}'");
    }
    else
    {
        dbProducts.Add(existing);
    }
}
Console.WriteLine($"Total Products in DB: {await productColl.CountDocumentsAsync(FilterDefinition<Product>.Empty)}");

// 4. Seed Carts (15+ items)
Console.WriteLine("\nSeeding Carts...");
var existingCartCount = await cartColl.CountDocumentsAsync(FilterDefinition<Cart>.Empty);
int cartsToCreate = Math.Max(0, 16 - (int)existingCartCount);
var rand = new Random();

for (int i = 0; i < cartsToCreate; i++)
{
    var cust = dbCustomers[rand.Next(dbCustomers.Count)];

    var cartItems = new List<CartItem>();
    int numItems = rand.Next(1, 4); // 1 to 3 items
    var selectedProds = dbProducts.OrderBy(_ => rand.Next()).Take(numItems).ToList();

    foreach (var prod in selectedProds)
    {
        cartItems.Add(new CartItem
        {
            ProductId = prod.ID!,
            ProductName = prod.Name,
            CategoryId = prod.CategoryId ?? "",
            UnitPrice = prod.Price,
            Quantity = rand.Next(1, 4),
            DiscountAmount = 0
        });
    }

    decimal subtotal = cartItems.Sum(item => item.UnitPrice * item.Quantity);
    decimal discount = 0;
    decimal tax = subtotal * 0.11m;
    decimal total = subtotal - discount + tax;

    var cart = new Cart
    {
        CustomerId = cust.ID,
        Items = cartItems,
        Subtotal = subtotal,
        DiscountAmount = discount,
        TaxAmount = tax,
        Total = total,
        IsCheckedOut = rand.Next(2) == 0
    };

    await cartColl.InsertOneAsync(cart);
    Console.WriteLine($"-> Created Cart for Customer {cust.FirstName} with {cartItems.Count} items.");
}
Console.WriteLine($"Total Carts in DB: {await cartColl.CountDocumentsAsync(FilterDefinition<Cart>.Empty)}");

// 5. Seed PricingRules (15+ items)
Console.WriteLine("\nSeeding Pricing Rules...");
var dummyRules = new List<PricingRule>();

// Category discounts
var snackCat = dbCategories.FirstOrDefault(c => c.Name == "Snack");
if (snackCat != null)
{
    dummyRules.Add(new PricingRule { RuleType = "DiscountPercentage", CategoryId = snackCat.ID, Condition = "CategoryName=Snack", Value = 10, Priority = 1 });
}
var bevCat = dbCategories.FirstOrDefault(c => c.Name == "Beverage");
if (bevCat != null)
{
    dummyRules.Add(new PricingRule { RuleType = "DiscountPercentage", CategoryId = bevCat.ID, Condition = "CategoryName=Beverage", Value = 5, Priority = 2 });
}
var freshCat = dbCategories.FirstOrDefault(c => c.Name == "Fresh");
if (freshCat != null)
{
    dummyRules.Add(new PricingRule { RuleType = "DiscountPercentage", CategoryId = freshCat.ID, Condition = "CategoryName=Fresh", Value = 15, Priority = 3 });
}
var dairyCat = dbCategories.FirstOrDefault(c => c.Name == "Dairy");
if (dairyCat != null)
{
    dummyRules.Add(new PricingRule { RuleType = "DiscountPercentage", CategoryId = dairyCat.ID, Condition = "CategoryName=Dairy", Value = 8, Priority = 4 });
}
var bakeryCat = dbCategories.FirstOrDefault(c => c.Name == "Bakery");
if (bakeryCat != null)
{
    dummyRules.Add(new PricingRule { RuleType = "DiscountPercentage", CategoryId = bakeryCat.ID, Condition = "CategoryName=Bakery", Value = 12, Priority = 5 });
}

// Product buy x get y
var oreoProd = dbProducts.FirstOrDefault(p => p.Name == "Oreo Pack");
if (oreoProd != null)
{
    dummyRules.Add(new PricingRule { RuleType = "BuyXGetY", ProductId = oreoProd.ID, Condition = "X=2,Y=1", Value = 0, Priority = 6 });
}
var cokeProd = dbProducts.FirstOrDefault(p => p.Name == "Coca-Cola Can");
if (cokeProd != null)
{
    dummyRules.Add(new PricingRule { RuleType = "BuyXGetY", ProductId = cokeProd.ID, Condition = "X=3,Y=1", Value = 0, Priority = 7 });
}
var breadProd = dbProducts.FirstOrDefault(p => p.Name == "Wholewheat Bread");
if (breadProd != null)
{
    dummyRules.Add(new PricingRule { RuleType = "BuyXGetY", ProductId = breadProd.ID, Condition = "X=2,Y=1", Value = 0, Priority = 8 });
}

// General rules and Tax rates
dummyRules.Add(new PricingRule { RuleType = "TaxRate", Condition = "CategoryName=Snack", Value = 11, Priority = 9 });
dummyRules.Add(new PricingRule { RuleType = "TaxRate", Condition = "CategoryName=Beverage", Value = 11, Priority = 10 });
dummyRules.Add(new PricingRule { RuleType = "TaxRate", Condition = "CategoryName=Fresh", Value = 5, Priority = 11 });
dummyRules.Add(new PricingRule { RuleType = "TaxRate", Condition = "CategoryName=Household", Value = 11, Priority = 12 });

// Payment fees
dummyRules.Add(new PricingRule { RuleType = "PaymentFee", Condition = "PaymentMethod=CreditCard", Value = 1.5m, Priority = 13 });
dummyRules.Add(new PricingRule { RuleType = "PaymentFee", Condition = "PaymentMethod=QRIS", Value = 0.7m, Priority = 14 });
dummyRules.Add(new PricingRule { RuleType = "PaymentFee", Condition = "PaymentMethod=EWallet", Value = 0.5m, Priority = 15 });
dummyRules.Add(new PricingRule { RuleType = "PaymentFee", Condition = "PaymentMethod=BankTransfer", Value = 0.3m, Priority = 16 });

foreach (var rule in dummyRules)
{
    var existing = await pricingRulesColl.Find(r => r.RuleType == rule.RuleType && r.Priority == rule.Priority && r.Condition == rule.Condition).FirstOrDefaultAsync();
    if (existing == null)
    {
        await pricingRulesColl.InsertOneAsync(rule);
        Console.WriteLine($"-> Created Pricing Rule: Type={rule.RuleType}, Priority={rule.Priority}, Cond='{rule.Condition}'");
    }
}
Console.WriteLine($"Total Pricing Rules in DB: {await pricingRulesColl.CountDocumentsAsync(FilterDefinition<PricingRule>.Empty)}");

// 6. Seed Payments (15+ items)
Console.WriteLine("\nSeeding Payments...");
var existingPaymentCount = await paymentColl.CountDocumentsAsync(FilterDefinition<Payment>.Empty);
int paymentsToCreate = Math.Max(0, 16 - (int)existingPaymentCount);

for (int i = 0; i < paymentsToCreate; i++)
{
    var cust = dbCustomers[rand.Next(dbCustomers.Count)];
    var method = (PaymentMethod)rand.Next(5);

    var payment = new Payment
    {
        Customer = cust,
        Date = DateTime.UtcNow.AddDays(-rand.Next(1, 30)),
        PaymentMethod = method
    };

    await paymentColl.InsertOneAsync(payment);
    Console.WriteLine($"-> Created Payment of type {method} for Customer {cust.FirstName}.");
}
Console.WriteLine($"Total Payments in DB: {await paymentColl.CountDocumentsAsync(FilterDefinition<Payment>.Empty)}");

// 7. Seed Receipts (15+ items)
Console.WriteLine("\nSeeding Receipts...");
var existingReceiptCount = await receiptColl.CountDocumentsAsync(FilterDefinition<Receipt>.Empty);
int receiptsToCreate = Math.Max(0, 16 - (int)existingReceiptCount);

for (int i = 0; i < receiptsToCreate; i++)
{
    var cust = dbCustomers[rand.Next(dbCustomers.Count)];

    var cartItems = new List<CartItem>();
    int numItems = rand.Next(1, 4);
    var selectedProds = dbProducts.OrderBy(_ => rand.Next()).Take(numItems).ToList();

    foreach (var prod in selectedProds)
    {
        cartItems.Add(new CartItem
        {
            ProductId = prod.ID!,
            ProductName = prod.Name,
            CategoryId = prod.CategoryId ?? "",
            UnitPrice = prod.Price,
            Quantity = rand.Next(1, 4),
            DiscountAmount = 0
        });
    }

    decimal subtotal = cartItems.Sum(item => item.UnitPrice * item.Quantity);
    decimal discount = 0;
    decimal tax = subtotal * 0.11m;
    decimal fee = subtotal * 0.01m;
    decimal total = subtotal - discount + tax + fee;
    var method = (PaymentMethod)rand.Next(5);

    var receipt = new Receipt
    {
        CustomerId = cust.ID,
        CustomerName = $"{cust.FirstName} {cust.LastName}",
        Items = cartItems,
        Subtotal = subtotal,
        DiscountAmount = discount,
        TaxAmount = tax,
        FeeAmount = fee,
        Total = total,
        PaymentMethod = method,
        Date = DateTime.UtcNow.AddDays(-rand.Next(1, 30))
    };

    await receiptColl.InsertOneAsync(receipt);
    Console.WriteLine($"-> Created Receipt for Customer {cust.FirstName} ({cust.LastName}) - Total: {total:N2} IDR");
}
Console.WriteLine($"Total Receipts in DB: {await receiptColl.CountDocumentsAsync(FilterDefinition<Receipt>.Empty)}");

Console.WriteLine("\nSeeding completed successfully!");
