using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Models.Dto;
using Tax.Matters.Web.Core.Modules.TaxManagement.Handlers;
using Tax.Matters.Web.Core.Modules.TaxManagement.Queries;

namespace Tax.Matters.Web.Pages.TaxManagement;

public class IndexModel(IMediator mediator, IConfiguration configuration) : PageModel
{
    private readonly IMediator _mediator = mediator;
    private readonly IConfiguration _configuration = configuration;
    public PageListDto<IncomeTax> Taxes { get; set; }

    public async Task OnGetAsync(
        int? pageIndex)
    {
        var pageSize = _configuration.GetValue("PageSize", 5);

        var query = new GetTaxCalculationTypesQuery(pageIndex ?? 1, pageSize);

        var result = await _mediator.Send(query);

        Taxes = result.Content ?? new PageListDto<IncomeTax>
        {
            Items = []
        };
    }
}
