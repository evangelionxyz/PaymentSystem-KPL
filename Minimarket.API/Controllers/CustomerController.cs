using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.Models;

namespace Minimarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController(CustomerService customerService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
    {
        var customers = await customerService.GetAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetById(string id)
    {
        var customer = await customerService.GetAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Create(Customer newCustomer)
    {
        if (string.IsNullOrWhiteSpace(newCustomer.Phone))
        {
            return BadRequest("Customer phone cannot be empty.");
        }

        var existing = await customerService.GetByPhoneAsync(newCustomer.Phone);
        if (existing != null)
        {
            return Conflict($"Customer with phone number '{newCustomer.Phone}' already exists.");
        }

        await customerService.CreateAsync(newCustomer);
        return CreatedAtAction(nameof(GetById), new { id = newCustomer.ID }, newCustomer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Customer updateCustomer)
    {
        var existingCustomer = await customerService.GetAsync(id);
        if (existingCustomer == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(updateCustomer.Phone))
        {
            return BadRequest("Customer phone cannot be empty.");
        }

        var duplicate = await customerService.GetByPhoneAsync(updateCustomer.Phone);
        if (duplicate != null && duplicate.ID != id)
        {
            return Conflict($"Customer with phone number '{updateCustomer.Phone}' already exists.");
        }

        updateCustomer.ID = existingCustomer.ID;
        await customerService.UpdateAsync(id, updateCustomer);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingCustomer = await customerService.GetAsync(id);
        if (existingCustomer == null)
        {
            return NotFound();
        }

        await customerService.RemoveAsync(id);
        return NoContent();
    }
}
