using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class ReceiptService(IOptions<Settings> settings, IMongoClient client)
{
    private readonly IMongoCollection<Receipt> _receipts =
        client.GetDatabase(settings.Value.DatabaseName)
              .GetCollection<Receipt>(settings.Value.ReceiptCollectionName);

    public async Task<List<Receipt>> GetAsync() =>
        await _receipts.Find(_ => true).ToListAsync();

    public async Task<Receipt?> GetAsync(string id) =>
        await _receipts.Find(x => x.ID == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Receipt receipt) =>
        await _receipts.InsertOneAsync(receipt);

    public async Task RemoveAsync(string id) =>
        await _receipts.DeleteOneAsync(x => x.ID == id);
}
