using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class CustomerService(IOptions<Settings> DatabaseSettings)
{
    private readonly IMongoCollection<Customer> _customerCollection = new MongoClient(DatabaseSettings.Value.ConnectionString)
        .GetDatabase(DatabaseSettings.Value.DatabaseName)
        .GetCollection<Customer>(DatabaseSettings.Value.CustomerCollectionName);

    public async Task<List<Customer>> GetAsync() => 
        await _customerCollection.Find(_ => true).ToListAsync();

    public async Task<Customer?> GetAsync(string id) => 
        await _customerCollection.Find(x => x.ID == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Customer newCustomer) =>
        await _customerCollection.InsertOneAsync(newCustomer);

    public async Task UpdateAsync(string id, Customer updateCustomer) => 
        await _customerCollection.ReplaceOneAsync(x => x.ID == id, updateCustomer);

    public async Task RemoveAsync(string id) => 
        await _customerCollection.DeleteOneAsync(x => x.ID == id);
}