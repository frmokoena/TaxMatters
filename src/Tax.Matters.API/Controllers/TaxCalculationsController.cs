using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tax.Matters.API.Core.Modules.TaxCalculations.Commands;
using Tax.Matters.API.Core.Modules.TaxCalculations.Models;
using Tax.Matters.API.Core.Modules.TaxCalculations.Queries;

namespace Tax.Matters.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("services/[controller]")]
    public class TaxCalculationsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> List(
            int pageNumber = 1,
            int limit = 20,
            string? keyword = null,
            string? sortOrder = null)
        {
            var filteringModel = new TaxCalculationsFilteringModel
            {
                Keyword = keyword,
                Limit = limit,
                PageNumber = pageNumber,
                SortOrder = sortOrder
            };

            var query = new GetTaxCalculationsQuery(filteringModel);

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaxCalculation(string id)
        {
            var query = new GetTaxCalculationQuery(id);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaxCalculation(string id)
        {
            var query = new DeleteTaxCalculationCommand(id);

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

        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate([FromBody] TaxCalculationRequestModel model)
        {
            var command = new CalculateTaxCommand(model);

            var response = await _mediator.Send(command);

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
}
