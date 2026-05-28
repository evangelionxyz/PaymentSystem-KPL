using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.States;

namespace Minimarket.API.Controllers;

/// <summary>
/// GET /api/config/machine-states — returns all FSM transition rows from MongoDB.
/// </summary>
[ApiController]
[Route("api/config")]
public class ConfigController(MachineStateService stateService) : ControllerBase
{
    [HttpGet("machine-states")]
    public async Task<ActionResult<IEnumerable<MachineStateTransition>>> GetMachineStates() =>
        Ok(await stateService.GetAllAsync());
}
