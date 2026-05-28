using Minimarket.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private static readonly Customer dummyCS = new() { FirstName = "Evangelion", LastName = "Manuhutu", Phone = "1234567890" };
    private static readonly List<Payment> _staticTestData =
    [
        new Payment {
            Date = DateTime.Now, 
            Customer = dummyCS,
            PaymentMethod = PaymentMethod.EWallet,
        },
    ];

    [HttpGet]
    public ActionResult<IEnumerable<Payment>> GetAll()
    {
        return Ok(_staticTestData);
    }

    [HttpGet("{id}")]
    public ActionResult GetById(string id)
    {
        var result = _staticTestData.Find(x => x.ID == id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }
}
