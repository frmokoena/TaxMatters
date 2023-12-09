using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Models.Dto;
using Tax.Matters.Web.Core.Modules.TaxManagement.Queries;

namespace Tax.Matters.Web.Pages.TaxManagement;

public class ProgressiveTableModel(IMediator mediator, IConfiguration configuration) : PageModel
{
    private readonly IMediator _mediator = mediator;
    private readonly IConfiguration _configuration = configuration;
    public PageListDto<ProgressiveIncomeTax> Taxes { get; set; } = default!;
    public string? Id { get; set; }

    public async Task OnGetAsync(
        string? id,
        int? pageIndex)
    {
        Id = Id ?? id;
        if (string.IsNullOrWhiteSpace(Id))
        {
            Taxes = new PageListDto<ProgressiveIncomeTax>
            {
                Items = []
            };

        }
        else
        {
            var pageSize = _configuration.GetValue("PageSize", 5);

            var query = new GetProgressiveTableQuery(Id, pageIndex ?? 1, pageSize);

            var result = await _mediator.Send(query);

            Taxes = result.Content ?? new PageListDto<ProgressiveIncomeTax>
            {
                Items = []
            };
        }
    }
}
