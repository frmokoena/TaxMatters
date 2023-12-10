using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Commands;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Queries;

namespace Tax.Matters.Web.Pages.TaxCalculations;

public class DeleteModel(IMediator mediator) : PageModel
{
    private readonly IMediator _mediator = mediator;

    [BindProperty]
    public TaxCalculation Calculation { get; set; } = default!;

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string? id, bool? saveChangesError = false)
    {
        if (id == null)
        {
            return NotFound();
        }

        var query = new GetTaxCalculationQuery(id);

        var result = await _mediator.Send(query);

        if (result.Content == null)
        {
            return NotFound();
        }

        Calculation = result.Content;

        if (saveChangesError.GetValueOrDefault())
        {
            ErrorMessage = $"Delete {id} failed. Try again";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var command = new DeleteTaxCalculationCommand(id);

        var result = await _mediator.Send(command);

        if (result.IsError && (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false))
        {
            return NotFound();
        }

        if (!result.IsError)
        {
            return RedirectToPage("./Index");
        }

        return RedirectToAction("./Delete",
                                 new { id, saveChangesError = true });
    }
}
