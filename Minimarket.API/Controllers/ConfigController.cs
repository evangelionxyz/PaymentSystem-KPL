using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.States;

namespace Minimarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController(MachineStateService stateService) : ControllerBase
{
    [HttpGet("machine-states")]
    public async Task<ActionResult<IEnumerable<MachineStateTransition>>> GetMachineStates() =>
        Ok(await stateService.GetAllAsync());
}
