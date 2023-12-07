using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tax.Matters.API.Controllers;

[Authorize(Policy = "web")]    
[ApiController]
[Route("services/[controller]")]
public class TaxCalculatorController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}
