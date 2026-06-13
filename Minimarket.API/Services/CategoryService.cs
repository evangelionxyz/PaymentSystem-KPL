using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class CategoryService(IOptions<Settings> settings, IMongoClient client)
{
    private readonly IMongoCollection<Category> _categoryCollection = client.GetDatabase(settings.Value.DatabaseName).GetCollection<Category>(settings.Value.CategoryCollectionName);
    public async Task<List<Category>> GetAsync() => await _categoryCollection.Find(_ => true).ToListAsync();
    public async Task<Category?> GetAsync(string id) => await _categoryCollection.Find(x => x.ID == id).FirstOrDefaultAsync();
    public async Task<Category?> GetByNameAsync(string name) => await _categoryCollection.Find(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
    public async Task CreateAsync(Category newCategory) => await _categoryCollection.InsertOneAsync(newCategory);
    public async Task UpdateAsync(string id, Category updateCategory) => await _categoryCollection.ReplaceOneAsync(x => x.ID == id, updateCategory);
    public async Task RemoveAsync(string id) => await _categoryCollection.DeleteOneAsync(x => x.ID == id);
}
