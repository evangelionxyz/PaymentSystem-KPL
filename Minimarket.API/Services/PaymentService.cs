using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class PaymentService(IOptions<Settings> databaseSettings)
{
    private readonly IMongoCollection<Payment> _paymentCollection = new MongoClient(databaseSettings.Value.ConnectionString)
        .GetDatabase(databaseSettings.Value.DatabaseName)
        .GetCollection<Payment>(databaseSettings.Value.PaymentCollectionName);

    public async Task<List<Payment>> GetAsync() =>
        await _paymentCollection.Find(_ => true).ToListAsync();

    public async Task<Payment?> GetAsync(string id) =>
        await _paymentCollection.Find(x => x.ID == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Payment newPayment) =>
        await _paymentCollection.InsertOneAsync(newPayment);

    public async Task UpdateAsync(string id, Payment updatePayment) =>
        await _paymentCollection.ReplaceOneAsync(x => x.ID == id, updatePayment);

    public async Task RemoveAsync(string id) =>
        await _paymentCollection.DeleteOneAsync(x => x.ID == id);
}
