using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.Models;

namespace Minimarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuditController(AuditLogService auditLogService) : ControllerBase
{
    [HttpPost("logs")]
    public async Task<ActionResult> LogTransition([FromBody] AuditLog log)
    {
        if (string.IsNullOrWhiteSpace(log.TransactionId) || string.IsNullOrWhiteSpace(log.FromState) || string.IsNullOrWhiteSpace(log.ToState))
        {
            return BadRequest("TransactionId, FromState, and ToState are required.");
        }

        log.Timestamp = DateTime.UtcNow;
        await auditLogService.CreateAsync(log);
        return Ok();
    }

    [HttpGet("logs/{transactionId}")]
    public async Task<ActionResult<IEnumerable<AuditLog>>> GetLogs(string transactionId)
    {
        var logs = await auditLogService.GetByTransactionIdAsync(transactionId);
        return Ok(logs);
    }

    [HttpGet("logs")]
    public async Task<ActionResult<IEnumerable<AuditLog>>> GetAllLogs()
    {
        var logs = await auditLogService.GetAllAsync();
        return Ok(logs);
    }
}
