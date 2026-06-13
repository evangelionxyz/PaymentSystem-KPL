using Minimarket.Core.States;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class MachineStateService(IOptions<Settings> settings, IMongoClient client)
{
    private readonly IMongoCollection<MachineStateTransition> _collection = client.GetDatabase(settings.Value.DatabaseName).GetCollection<MachineStateTransition>(settings.Value.MachineStateCollectionName);
    public async Task<List<MachineStateTransition>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
}
