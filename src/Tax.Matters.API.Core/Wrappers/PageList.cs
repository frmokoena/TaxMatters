using Microsoft.EntityFrameworkCore;

namespace Tax.Matters.API.Core.Wrappers
{
    public class PageList<T>
    {
        public IEnumerable<T> Items { get; }
        public int PageIndex { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }
        public int Limit { get; }

        public PageList(
            List<T> items,
            int count,
            int pageIndex,
            int limit)
        {
            PageIndex = pageIndex;
            Limit = limit;
            TotalPages = (int)Math.Ceiling(count / (double)limit);
            TotalCount = count;
            Items = items;
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PageList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();

            var query = source.Skip((pageIndex - 1) * pageSize).Take(pageSize);


            var items = await query.ToListAsync();

            return new PageList<T>(items, count, pageIndex, pageSize);
        }

        public static async Task<PageList<T>> CreateAsync(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return await Task.FromResult(new PageList<T>(items, count, pageIndex, pageSize));
        }
    }
}
