namespace Tax.Matters.API.Core.Modules.TaxManagement.Models;

/// <summary>
/// Class <c>TaxCalculationTypesFilteringModel</c> models the filtering for the progressive list query
/// </summary>
public class TaxCalculationTypesFilteringModel
{
    public int Limit { get; set; }
    public int PageNumber { get; set; }
}
