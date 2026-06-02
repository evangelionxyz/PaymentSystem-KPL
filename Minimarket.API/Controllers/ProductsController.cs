using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.Models;

namespace Minimarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(ProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await productService.GetAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(string id)
    {
        var product = await productService.GetAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product newProduct)
    {
        if (string.IsNullOrWhiteSpace(newProduct.Name))
        {
            return BadRequest("Product name cannot be empty.");
        }

        var existing = await productService.GetByNameAsync(newProduct.Name);
        if (existing != null)
        {
            return Conflict($"Product with name '{newProduct.Name}' already exists.");
        }

        await productService.CreateAsync(newProduct);
        return CreatedAtAction(nameof(GetById), new { id = newProduct.ID }, newProduct);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Product updateProduct)
    {
        var existingProduct = await productService.GetAsync(id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(updateProduct.Name))
        {
            return BadRequest("Product name cannot be empty.");
        }

        var duplicate = await productService.GetByNameAsync(updateProduct.Name);
        if (duplicate != null && duplicate.ID != id)
        {
            return Conflict($"Product with name '{updateProduct.Name}' already exists.");
        }

        updateProduct.ID = existingProduct.ID;
        await productService.UpdateAsync(id, updateProduct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingProduct = await productService.GetAsync(id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        await productService.RemoveAsync(id);
        return NoContent();
    }
}


