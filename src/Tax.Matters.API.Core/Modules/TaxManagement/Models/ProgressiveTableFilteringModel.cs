namespace Tax.Matters.API.Core.Modules.TaxManagement.Models;

/// <summary>
/// Class <c>ProgressiveTableFilteringModel</c> models the filtering for the progressive list query
/// </summary>
public class ProgressiveTableFilteringModel
{
    public string IncomeTaxId { get; set; } = default!;
    public int Limit { get; set; }
    public int PageNumber { get; set; }
}
