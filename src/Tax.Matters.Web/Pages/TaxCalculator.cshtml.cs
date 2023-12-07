using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tax.Matters.Web.Core.Modules.TaxCalculation.Models;

namespace Tax.Matters.Web.Pages;

public class TaxCalculatorModel : PageModel
{
    [BindProperty]
    public TaxCalculationInputModel Input { get; set; } = default!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.AddModelError("PostalCode", "Not found");
        return Page();
    }
}
