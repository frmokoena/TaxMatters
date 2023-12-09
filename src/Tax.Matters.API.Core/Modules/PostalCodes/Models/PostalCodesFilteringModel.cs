namespace Tax.Matters.API.Core.Modules.PostalCodes.Models
{
    public class PostalCodesFilteringModel
    {
        public int Limit { get; set; }
        public int PageNumber { get; set; }
        public string? Keyword { get; set; }
    }
}
