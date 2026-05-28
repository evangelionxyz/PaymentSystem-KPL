using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class CartService(IOptions<Settings> databaseSettings)
{
    private readonly IMongoCollection<Cart> _cartCollection = new MongoClient(databaseSettings.Value.ConnectionString)
        .GetDatabase(databaseSettings.Value.DatabaseName)
        .GetCollection<Cart>(databaseSettings.Value.CartCollectionName);

    public async Task<List<Cart>> GetAsync() =>
        await _cartCollection.Find(_ => true).ToListAsync();

    public async Task<Cart?> GetAsync(string id) =>
        await _cartCollection.Find(x => x.ID == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Cart newCart) =>
        await _cartCollection.InsertOneAsync(newCart);

    public async Task UpdateAsync(string id, Cart updateCart) =>
        await _cartCollection.ReplaceOneAsync(x => x.ID == id, updateCart);

    public async Task RemoveAsync(string id) =>
        await _cartCollection.DeleteOneAsync(x => x.ID == id);
}
