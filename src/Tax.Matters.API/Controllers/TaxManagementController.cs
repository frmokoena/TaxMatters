using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tax.Matters.API.Core.Modules.TaxManagement.Models;
using Tax.Matters.API.Core.Modules.TaxManagement.Queries;

namespace Tax.Matters.API.Controllers;

[ApiController]
[Authorize(Policy = "Web")]
[Route("services/[controller]")]
public class TaxManagementController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("taxcalculationtypes")]
    public async Task<IActionResult> List(int pageNumber = 1, int limit = 20)
    {
        var filteringModel = new TaxCalculationTypesFilteringModel
        {
            Limit = limit,
            PageNumber = pageNumber
        };

        var query = new GetTaxCalculationTypesQuery(filteringModel);

        var response = await _mediator.Send(query);

        if (!response.IsError)
        {
            return Ok(response.Content);
        }

        if (!string.IsNullOrWhiteSpace(response.HttpReasonPhrase))
        {
            return StatusCode((int)response.HttpStatusCode, new
            {
                error = response.HttpReasonPhrase
            });
        }

        return StatusCode((int)response.HttpStatusCode);
    }

    [HttpGet("progressive")]
    public async Task<IActionResult> ProgressiveTable(
        string incomeTaxId,
        int pageNumber = 1, 
        int limit = 20)
    {
        var filteringModel = new ProgressiveTableFilteringModel
        {
            IncomeTaxId = incomeTaxId,
            Limit = limit,
            PageNumber = pageNumber
        };

        var query = new GetProgressiveTableQuery(filteringModel);

        var response = await _mediator.Send(query);

        if (!response.IsError)
        {
            return Ok(response.Content);
        }

        if (!string.IsNullOrWhiteSpace(response.HttpReasonPhrase))
        {
            return StatusCode((int)response.HttpStatusCode, new
            {
                error = response.HttpReasonPhrase
            });
        }

        return StatusCode((int)response.HttpStatusCode);
    }
}
