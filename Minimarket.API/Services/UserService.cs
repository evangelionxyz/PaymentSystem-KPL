using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class UserService(IOptions<Settings> settings, IMongoClient client)
{
    private readonly IMongoCollection<User> _userCollection =
        client.GetDatabase(settings.Value.DatabaseName)
              .GetCollection<User>(settings.Value.UserCollectionName);

    public async Task<List<User>> GetAsync() => 
        await _userCollection.Find(_ => true).ToListAsync();

    public async Task<User?> GetAsync(string id) => 
        await _userCollection.Find(x => x.ID == id).FirstOrDefaultAsync();

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _userCollection.Find(x => x.Username.ToLower() == username.ToLower()).FirstOrDefaultAsync();

    public async Task CreateAsync(User newUser) =>
        await _userCollection.InsertOneAsync(newUser);

    public async Task UpdateAsync(string id, User updateUser) => 
        await _userCollection.ReplaceOneAsync(x => x.ID == id, updateUser);

    public async Task RemoveAsync(string id) => 
        await _userCollection.DeleteOneAsync(x => x.ID == id);
}
