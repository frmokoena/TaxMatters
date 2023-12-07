namespace Tax.Matters.Web.Core.Models.Dto;

public class PageListDto<T>
{
    public IEnumerable<T> Items { get; } = default!;
    public int PageIndex { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public int Limit { get; }
}
