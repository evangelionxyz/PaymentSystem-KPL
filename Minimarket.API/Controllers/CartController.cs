using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.Models;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController(CartService cartService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cart>>> GetAll() => Ok(await cartService.GetAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Cart>> GetById(string id)
    {
        var cart = await cartService.GetAsync(id);
        return cart is null ? NotFound() : Ok(cart);
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
        var existing = await cartService.GetAsync(id);
        if (existing is null) return NotFound();
        updateCart.ID = existing.ID;
        await cartService.UpdateAsync(id, updateCart);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await cartService.GetAsync(id);
        if (existing is null) return NotFound();
        await cartService.RemoveAsync(id);
        return NoContent();
    }

    // Business Endpoints (Task 7.2)
    public record AddItemRequest(string CartId, string ProductId, int Quantity);
    public record RemoveItemRequest(string CartId, string ProductId);
    public record CheckoutRequest(string CartId);

    /// <summary>POST /api/cart/add — adds a product to a cart.</summary>
    [HttpPost("add")]
    public async Task<ActionResult<Cart>> AddItem([FromBody] AddItemRequest req)
    {
        try
        {
            var cart = await cartService.AddItemAsync(req.CartId, req.ProductId, req.Quantity);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>POST /api/cart/remove — removes a product from a cart.</summary>
    [HttpPost("remove")]
    public async Task<ActionResult<Cart>> RemoveItem([FromBody] RemoveItemRequest req)
    {
        try
        {
            var cart = await cartService.RemoveItemAsync(req.CartId, req.ProductId);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>POST /api/cart/checkout — applies pricing rules and returns priced cart.</summary>
    [HttpPost("checkout")]
    public async Task<ActionResult<Cart>> Checkout([FromBody] CheckoutRequest req)
    {
        try
        {
            var cart = await cartService.CheckoutAsync(req.CartId);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>GET /api/cart/pending — returns all checked-out but unpaid carts.</summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<Cart>>> GetPending()
    {
        var all = await cartService.GetAsync();
        var pending = all.Where(c => c.IsCheckedOut && !c.IsPaid && c.Items.Count > 0);
        return Ok(pending);
    }
}
