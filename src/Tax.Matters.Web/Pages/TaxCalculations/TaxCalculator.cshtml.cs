using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Commands;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Models;

namespace Tax.Matters.Web.Pages.TaxCalculations;

public class TaxCalculatorModel(IMediator mediator) : PageModel
{
    private readonly IMediator _mediator = mediator;

    [BindProperty]
    public TaxCalculationInputModel Calculation { get; set; } = default!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var calculation = new TaxCalculationInputModel();

        if (await TryUpdateModelAsync(
            calculation,
            "calculation",   // Prefix for form value.
            m => m.AnnualIncome, m => m.PostalCode))
        {
            var command = new CalculateTaxCommand(calculation);

            var result = await _mediator.Send(command);

            if (!result.IsError)
            {
                return RedirectToPage("./Index");
            }

            if (!string.IsNullOrWhiteSpace(result.Error))
            {
                // We can intercept the error to present user friendly message and possibly redact sensitive information
                ModelState.AddModelError("", result.Error);
            }
            else
            {
                ModelState.AddModelError("", "Unexpected response received while executing the request");
            }                
        }

        return Page();
    }
}
