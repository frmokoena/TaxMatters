using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tax.Matters.Web.Core.Modules.TaxCalculation.Models;

namespace Tax.Matters.Web.Pages.TaxCalculations
{
    public class TaxCalculatorModel : PageModel
    {
        [BindProperty]
        public TaxCalculationInputModel Input { get; set; } = default!;

        public void OnGet()
        {
        }
    }
}
