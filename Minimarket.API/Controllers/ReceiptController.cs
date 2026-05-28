using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.Models;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptController(ReceiptService receiptService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Receipt>>> GetAll() =>
        Ok(await receiptService.GetAsync());

    /// <summary>GET /api/receipts/{id} — returns 404 for missing receipts.</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Receipt>> GetById(string id)
    {
        var receipt = await receiptService.GetAsync(id);
        return receipt is null ? NotFound() : Ok(receipt);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await receiptService.GetAsync(id);
        if (existing is null) return NotFound();
        await receiptService.RemoveAsync(id);
        return NoContent();
    }
}
