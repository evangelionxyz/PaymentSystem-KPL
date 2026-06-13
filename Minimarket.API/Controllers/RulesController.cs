using Microsoft.AspNetCore.Mvc;
using Minimarket.API.Services;
using Minimarket.Core.Models;
using Microsoft.Extensions.Options;

namespace Minimarket.API.Controllers;

/// <summary>
/// GET /api/rules/pricing  — returns all pricing rules from MongoDB.
/// GET /api/rules/tax      — returns current TaxSettings (from appsettings).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RulesController(PricingRuleService ruleService,
    IOptions<TaxSettings> taxSettings) : ControllerBase
{
    [HttpGet("pricing")]
    public async Task<ActionResult<IEnumerable<PricingRule>>> GetPricingRules() =>
        Ok(await ruleService.GetAllAsync());

    [HttpGet("tax")]
    public ActionResult<TaxSettings> GetTaxSettings() =>
        Ok(taxSettings.Value);
}
