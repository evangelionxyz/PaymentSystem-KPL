using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minimarket.Core.Models;

namespace Minimarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAll()
    {
        return NotFound();
    }

    [HttpGet("{id}")]
    public ActionResult GetById(string id)
    {
        return NotFound();
    }
}


