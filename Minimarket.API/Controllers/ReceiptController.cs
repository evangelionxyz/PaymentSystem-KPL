using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.Models;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptController(ReceiptService receiptService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Receipt>>> GetAll()
    {
        var receipts = await receiptService.GetAsync();
        return Ok(receipts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Receipt>> GetById(string id)
    {
        var receipt = await receiptService.GetAsync(id);
        if (receipt == null)
        {
            return NotFound();
        }

        return Ok(receipt);
    }

    [HttpPost]
    public async Task<ActionResult<Receipt>> Create(Receipt newReceipt)
    {
        await receiptService.CreateAsync(newReceipt);
        return CreatedAtAction(nameof(GetById), new { id = newReceipt.ID }, newReceipt);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Receipt updateReceipt)
    {
        var existingReceipt = await receiptService.GetAsync(id);
        if (existingReceipt == null)
        {
            return NotFound();
        }

        updateReceipt.ID = existingReceipt.ID;
        await receiptService.UpdateAsync(id, updateReceipt);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingReceipt = await receiptService.GetAsync(id);
        if (existingReceipt == null)
        {
            return NotFound();
        }

        await receiptService.RemoveAsync(id);
        return NoContent();
    }
}
