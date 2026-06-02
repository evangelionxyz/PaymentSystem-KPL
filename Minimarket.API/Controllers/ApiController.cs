using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Minimarket.API.Controllers;

[Route("api/")]
[ApiController]
public class ApiController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ApiController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public ActionResult GetAll()
    {
        var request = HttpContext.Request;
        var process = Process.GetCurrentProcess();
        var connectionString = _configuration["Database:ConnectionString"];
        var password = _configuration["Database:Password"];

        return Ok(new
        {
            Host = request.Host.Value,
            Scheme = request.Scheme,
            PathBase = request.PathBase.Value,
            Database = new
            {
                ConnectionString = connectionString,
                PasswordSet = !string.IsNullOrWhiteSpace(password)
            },
            Server = new
            {
                Environment.MachineName,
                OS = RuntimeInformation.OSDescription,
                Framework = RuntimeInformation.FrameworkDescription,
                ProcessId = process.Id,
                WorkingSetBytes = process.WorkingSet64
            }
        });
    }
}
