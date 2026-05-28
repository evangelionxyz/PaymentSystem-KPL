using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class ProductService(IOptions<Settings> databaseSettings)
{
    private readonly IMongoCollection<Product> _productCollection = new MongoClient(databaseSettings.Value.ConnectionString)
        .GetDatabase(databaseSettings.Value.DatabaseName)
        .GetCollection<Product>(databaseSettings.Value.ProductCollectionName);

    public async Task<List<Product>> GetAsync() =>
        await _productCollection.Find(_ => true).ToListAsync();

    public async Task<Product?> GetAsync(string id) =>
        await _productCollection.Find(x => x.ID == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Product newProduct) =>
        await _productCollection.InsertOneAsync(newProduct);

    public async Task UpdateAsync(string id, Product updateProduct) =>
        await _productCollection.ReplaceOneAsync(x => x.ID == id, updateProduct);

    public async Task RemoveAsync(string id) =>
        await _productCollection.DeleteOneAsync(x => x.ID == id);
}
