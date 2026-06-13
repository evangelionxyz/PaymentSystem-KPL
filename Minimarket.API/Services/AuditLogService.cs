using Minimarket.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Minimarket.API.Services;

public class AuditLogService(IOptions<Settings> settings, IMongoClient client)
{
    private readonly IMongoCollection<AuditLog> _logCollection = client.GetDatabase(settings.Value.DatabaseName).GetCollection<AuditLog>(settings.Value.AuditLogCollectionName);
    public async Task<List<AuditLog>> GetByTransactionIdAsync(string transactionId) =>
        await _logCollection.Find(x => x.TransactionId == transactionId)
                            .SortBy(x => x.Timestamp)
                            .ToListAsync();
    public async Task CreateAsync(AuditLog log) => await _logCollection.InsertOneAsync(log);
    public async Task<List<AuditLog>> GetAllAsync() => await _logCollection.Find(_ => true).SortBy(x => x.Timestamp).ToListAsync();
}
