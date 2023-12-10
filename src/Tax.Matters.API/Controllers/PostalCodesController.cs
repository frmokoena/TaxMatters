using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tax.Matters.API.Core.Modules.PostalCodes.Commands;
using Tax.Matters.API.Core.Modules.PostalCodes.Models;
using Tax.Matters.API.Core.Modules.PostalCodes.Queries;
using Tax.Matters.Client;

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

        if (response.ResponseError == ResponseError.Http)
        {
            if(!string.IsNullOrWhiteSpace(response.Raw))
            {
                return StatusCode((int)response.HttpStatusCode, response.Raw);
            }
            else if (!string.IsNullOrWhiteSpace(response.Error))
            {
                return StatusCode((int)response.HttpStatusCode, response.Error);
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

    [HttpPost]
    public async Task<IActionResult> Calculate([FromBody] CreatePostalCodeRequestModel model)
    {
        var command = new CreatePostalCodeCommand(model);

        var response = await _mediator.Send(command);

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
                return StatusCode((int)response.HttpStatusCode, response.Error);
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