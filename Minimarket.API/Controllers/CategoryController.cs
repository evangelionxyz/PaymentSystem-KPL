using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.Models;

namespace Minimarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(CategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll()
    {
        var categories = await categoryService.GetAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetById(string id)
    {
        var category = await categoryService.GetAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Create(Category newCategory)
    {
        if (string.IsNullOrWhiteSpace(newCategory.Name))
        {
            return BadRequest("Category name cannot be empty.");
        }

        var existing = await categoryService.GetByNameAsync(newCategory.Name);
        if (existing != null)
        {
            return Conflict($"Category with name '{newCategory.Name}' already exists.");
        }

        await categoryService.CreateAsync(newCategory);
        return CreatedAtAction(nameof(GetById), new { id = newCategory.ID }, newCategory);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Category updateCategory)
    {
        var existingCategory = await categoryService.GetAsync(id);
        if (existingCategory == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(updateCategory.Name))
        {
            return BadRequest("Category name cannot be empty.");
        }

        var duplicate = await categoryService.GetByNameAsync(updateCategory.Name);
        if (duplicate != null && duplicate.ID != id)
        {
            return Conflict($"Category with name '{updateCategory.Name}' already exists.");
        }

        updateCategory.ID = existingCategory.ID;
        await categoryService.UpdateAsync(id, updateCategory);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existingCategory = await categoryService.GetAsync(id);
        if (existingCategory == null)
        {
            return NotFound();
        }

        await categoryService.RemoveAsync(id);
        return NoContent();
    }
}
