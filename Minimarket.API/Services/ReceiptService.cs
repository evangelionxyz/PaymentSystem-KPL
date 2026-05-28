using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class ReceiptService(IOptions<Settings> databaseSettings)
{
    private readonly IMongoCollection<Receipt> _receiptCollection = new MongoClient(databaseSettings.Value.ConnectionString)
        .GetDatabase(databaseSettings.Value.DatabaseName)
        .GetCollection<Receipt>(databaseSettings.Value.ReceiptCollectionName);

    public async Task<List<Receipt>> GetAsync() =>
        await _receiptCollection.Find(_ => true).ToListAsync();

    public async Task<Receipt?> GetAsync(string id) =>
        await _receiptCollection.Find(x => x.ID == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Receipt newReceipt) =>
        await _receiptCollection.InsertOneAsync(newReceipt);

    public async Task UpdateAsync(string id, Receipt updateReceipt) =>
        await _receiptCollection.ReplaceOneAsync(x => x.ID == id, updateReceipt);

    public async Task RemoveAsync(string id) =>
        await _receiptCollection.DeleteOneAsync(x => x.ID == id);
}
