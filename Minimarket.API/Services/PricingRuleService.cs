using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class PricingRuleService(IOptions<Settings> settings, IMongoClient client)
{
    private readonly IMongoCollection<PricingRule> _collection =
        client.GetDatabase(settings.Value.DatabaseName)
              .GetCollection<PricingRule>(settings.Value.PricingRuleCollectionName);

    public async Task<List<PricingRule>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<PricingRule?> GetByIdAsync(string id) =>
        await _collection.Find(r => r.ID == id).FirstOrDefaultAsync();

    public async Task CreateAsync(PricingRule rule) =>
        await _collection.InsertOneAsync(rule);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(r => r.ID == id);
}
