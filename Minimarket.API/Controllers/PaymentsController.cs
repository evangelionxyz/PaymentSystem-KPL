using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.Models;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController(PaymentService paymentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payment>>> GetAll() =>
        Ok(await paymentService.GetAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Payment>> GetById(string id)
    {
        var payment = await paymentService.GetAsync(id);
        return payment is null ? NotFound() : Ok(payment);
    }

    public record ProcessPaymentRequest(string CartId, PaymentMethod Method, string? CustomerId);

    /// <summary>POST /api/payment — process payment, create receipt, return it.</summary>
    [HttpPost]
    public async Task<ActionResult<Receipt>> Process([FromBody] ProcessPaymentRequest req)
    {
        try
        {
            var receipt = await paymentService.ProcessAsync(req.CartId, req.Method, req.CustomerId);
            return CreatedAtResult(receipt);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    private CreatedResult CreatedAtResult(Receipt receipt) =>
        Created($"/api/receipts/{receipt.ID}", receipt);
}
