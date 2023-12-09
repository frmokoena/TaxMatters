namespace Tax.Matters.API.Core.Modules.TaxCalculations.Models;

/// <summary>
/// Class <c>TaxCalculationsFiltering</c> models the filtering of the tax calculations list query
/// </summary>
public class TaxCalculationsFilteringModel
{
    public int Limit { get; set; }
    public int PageNumber { get; set; }
    public string? Keyword { get; set; }
    public string? SortOrder {  get; set; }
}
