using Microsoft.EntityFrameworkCore;

namespace Tax.Matters.API.Core.Wrappers;

public class PageList<T> : List<T>
{
    public PageList(IEnumerable<T> items, int count, int pageIndex, int pageSize = 20)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        AddRange(items);
    }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }

    public static async Task<PageList<T>> CreateAsync(
        IQueryable<T> source, int pageIndex, int pageSize = 20)
    {
        var count = await source.CountAsync();
        var items = await source.Skip(
            (pageIndex - 1) * pageSize)
            .Take(pageSize).ToListAsync();
        return new PageList<T>(items, count, pageIndex, pageSize);
    }
}
