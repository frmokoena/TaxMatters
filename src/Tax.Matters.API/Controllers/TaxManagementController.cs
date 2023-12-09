using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tax.Matters.API.Core.Modules.TaxManagement.Models;
using Tax.Matters.API.Core.Modules.TaxManagement.Queries;
using Tax.Matters.Client;

namespace Tax.Matters.API.Controllers;

[ApiController]
[Authorize(Policy = "Web")]
[Route("services/[controller]")]
public class TaxManagementController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("taxcalculationtypes")]
    public async Task<IActionResult> ListTaxCalculationTypes(int pageNumber = 1, int limit = 20)
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

        if (response.ResponseError == ResponseError.Http)
        {
            if (!string.IsNullOrWhiteSpace(response.Raw))
            {
                return StatusCode((int)response.HttpStatusCode, response.Raw);
            }
            else if (!string.IsNullOrWhiteSpace(response.Error))
            {
                return StatusCode((int)response.HttpStatusCode, response.Raw);
            }
            else
            {
                return StatusCode((int)response.HttpStatusCode);
            }
        }

        if (!string.IsNullOrWhiteSpace(response.Error))
        {
            return StatusCode(500, response.Error);
        }

        return StatusCode(500, "unexpected response received while executing the request");
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

        if (response.ResponseError == ResponseError.Http)
        {
            if (!string.IsNullOrWhiteSpace(response.Raw))
            {
                return StatusCode((int)response.HttpStatusCode, response.Raw);
            }
            else if (!string.IsNullOrWhiteSpace(response.Error))
            {
                return StatusCode((int)response.HttpStatusCode, response.Raw);
            }
            else
            {
                return StatusCode((int)response.HttpStatusCode);
            }
        }

        if (!string.IsNullOrWhiteSpace(response.Error))
        {
            return StatusCode(500, response.Error);
        }

        return StatusCode(500, "unexpected response received while executing the request");
    }
}
