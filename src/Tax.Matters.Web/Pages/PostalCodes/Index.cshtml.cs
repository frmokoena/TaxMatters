using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Models.Dto;
using Tax.Matters.Web.Core.Modules.PostalCodes.Queries;

namespace Tax.Matters.Web.Pages.PostalCodes;

public class IndexModel(IMediator mediator, IConfiguration configuration) : PageModel
{
    private readonly IMediator _mediator = mediator;
    private readonly IConfiguration _configuration = configuration;
    public string? CurrentFilter { get; set; }
    public string? CurrentSort { get; set; }

    public PageListDto<PostalCode> PostalCodes { get; set; }

    public async Task OnGetAsync(
        string currentFilter,
        string searchString,
        int? pageIndex)
    {
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

        var query = new GetPostalCodesQuery(searchString, pageIndex ?? 1, pageSize);

        var result = await _mediator.Send(query);

        PostalCodes = result.Content ?? new PageListDto<PostalCode>
        {
            Items = []
        };
    }
}
