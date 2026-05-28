using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.Models;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController(PaymentService paymentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payment>>> GetAll()
    {
        var payments = await paymentService.GetAsync();
        return Ok(payments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Payment>> GetById(string id)
    {
        var payment = await paymentService.GetAsync(id);
        if (payment == null)
        {
            return NotFound();
        }

        return Ok(payment);
    }

    [HttpPost]
    public async Task<ActionResult<Payment>> Create(Payment newPayment)
    {
        await paymentService.CreateAsync(newPayment);
        return CreatedAtAction(nameof(GetById), new { id = newPayment.ID }, newPayment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Payment updatePayment)
    {
        var existingPayment = await paymentService.GetAsync(id);
        if (existingPayment == null)
        {
            return NotFound();
        }

        updatePayment.ID = existingPayment.ID;
        await paymentService.UpdateAsync(id, updatePayment);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingPayment = await paymentService.GetAsync(id);
        if (existingPayment == null)
        {
            return NotFound();
        }

        await paymentService.RemoveAsync(id);
        return NoContent();
    }
}
