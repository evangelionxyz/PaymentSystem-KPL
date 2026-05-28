using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.Models;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController(CartService cartService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cart>>> GetAll()
    {
        var carts = await cartService.GetAsync();
        return Ok(carts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cart>> GetById(string id)
    {
        var cart = await cartService.GetAsync(id);
        if (cart == null)
        {
            return NotFound();
        }

        return Ok(cart);
    }

    [HttpPost]
    public async Task<ActionResult<Cart>> Create(Cart newCart)
    {
        await cartService.CreateAsync(newCart);
        return CreatedAtAction(nameof(GetById), new { id = newCart.ID }, newCart);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Cart updateCart)
    {
        var existingCart = await cartService.GetAsync(id);
        if (existingCart == null)
        {
            return NotFound();
        }

        updateCart.ID = existingCart.ID;
        await cartService.UpdateAsync(id, updateCart);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingCart = await cartService.GetAsync(id);
        if (existingCart == null)
        {
            return NotFound();
        }

        await cartService.RemoveAsync(id);
        return NoContent();
    }
}
