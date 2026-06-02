using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.Models;

namespace Minimarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(UserService userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
        {
            return BadRequest("Username, and password.");
        }

        var existing = await userService.GetByUsernameAsync(user.Username);
        if (existing != null)
        {
            return Conflict($"User '{user.Username}' already exists.");
        }

        await userService.CreateAsync(user);
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
        {
            return BadRequest("Username and password are required.");
        }

        var user = await userService.GetByUsernameAsync(req.Username);
        if (user == null || user.Password != req.Password)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(user);
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
