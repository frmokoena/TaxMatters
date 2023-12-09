namespace Tax.Matters.API.Core.Modules.PostalCodes.Models
{
    /// <summary>
    /// Class <c>PostalCodesFilteringModel</c> models the filtering for the postal codes query
    /// </summary>
    public class PostalCodesFilteringModel
    {
        public int Limit { get; set; }
        public int PageNumber { get; set; }
        public string? Keyword { get; set; }
    }
}
