using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tax.Matters.API.Core.Modules.PostalCodes.Models;
using Tax.Matters.API.Core.Modules.PostalCodes.Queries;

namespace Tax.Matters.API.Controllers;

[Authorize]
[ApiController]
[Route("services/[controller]")]
public class PostalCodesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> List(
        int pageNumber = 1,
        int limit = 20,
        string? keyword = null)
    {
        var filteringModel = new PostalCodesFilteringModel
        {
            Keyword = keyword,
            Limit = limit,
            PageNumber = pageNumber
        };

        var query = new GetPostalCodesQuery(filteringModel);

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