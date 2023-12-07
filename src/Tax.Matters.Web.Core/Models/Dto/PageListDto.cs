namespace Tax.Matters.Web.Core.Models.Dto;

public class PageListDto<T> : List<T>
{
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }
}
