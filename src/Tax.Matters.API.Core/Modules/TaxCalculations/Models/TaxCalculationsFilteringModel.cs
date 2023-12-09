namespace Tax.Matters.API.Core.Modules.TaxCalculations.Models;

public class TaxCalculationsFilteringModel
{
    public int Limit { get; set; }
    public int PageNumber { get; set; }
    public string? Keyword { get; set; }
    public string? SortOrder {  get; set; }
}
