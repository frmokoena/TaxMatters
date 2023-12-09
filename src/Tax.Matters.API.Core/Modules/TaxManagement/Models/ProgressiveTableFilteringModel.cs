namespace Tax.Matters.API.Core.Modules.TaxManagement.Models;

public class ProgressiveTableFilteringModel
{
    public string IncomeTaxId { get; set; } = default!;
    public int Limit { get; set; }
    public int PageNumber { get; set; }
}
