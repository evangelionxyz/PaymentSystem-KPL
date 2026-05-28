using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class CustomerService(IOptions<Settings> customerDatabaseSettings)
{
    private readonly IMongoCollection<Customer> _customerCollection = new MongoClient(customerDatabaseSettings.Value.ConnectionString)
        .GetDatabase(customerDatabaseSettings.Value.DatabaseName)
        .GetCollection<Customer>(customerDatabaseSettings.Value.CustomerCollectionName);

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