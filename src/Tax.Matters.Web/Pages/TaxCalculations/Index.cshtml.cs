using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Models.Dto;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Queries;

namespace Tax.Matters.Web.Pages.TaxCalculations;

public class IndexModel(IMediator mediator, IConfiguration configuration) : PageModel
{
    private readonly IMediator _mediator = mediator;
    private readonly IConfiguration _configuration = configuration;

    public string? IncomeSort { get; set; }
    public string? DateSort { get; set; }
    public string? CurrentFilter { get; set; }
    public string? CurrentSort { get; set; }

    public PageListDto<TaxCalculation> Calculations { get; set; } = default!;

    public async Task OnGetAsync(
        string sortOrder,
        string currentFilter, 
        string searchString, 
        int? pageIndex)
    {
        CurrentSort = sortOrder;
        DateSort = string.IsNullOrEmpty(sortOrder) ? "date_asc" : "";

        IncomeSort = sortOrder == "Income" ? "income_desc" : "Income";

        if (searchString != null)
        {
            pageIndex = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        CurrentFilter = searchString;

        var pageSize = _configuration.GetValue("PageSize", 5);

        var query = new GetTaxCalculationsQuery(searchString, sortOrder,pageIndex ?? 1, pageSize);

        var result = await _mediator.Send(query);

        Calculations = result.Content ?? new PageListDto<TaxCalculation>
        {
            Items = []
        };       
    }
}
