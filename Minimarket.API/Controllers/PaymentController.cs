using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private static readonly List<PaymentModel> _staticTestData =
    [
        new PaymentModel{Id = new Guid().ToString(), Username = "Test Name A" },
        new PaymentModel{Id = new Guid().ToString(), Username = "Test Name B" },
        new PaymentModel{Id = new Guid().ToString(), Username = "Test Name C" },
        new PaymentModel{Id = new Guid().ToString(), Username = "Test Name D" },
        new PaymentModel{Id = new Guid().ToString(), Username = "Test Name E" },
    ];

    [HttpGet]
    public ActionResult<IEnumerable<PaymentModel>> GetAll()
    {
        return Ok(_staticTestData);
    }

    [HttpGet("{id}")]
    public ActionResult GetById(string id)
    {
        var result = _staticTestData.Find(x => x.Id == id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }
}
