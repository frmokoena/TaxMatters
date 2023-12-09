using System.Linq.Expressions;

namespace Tax.Matters.API.Core;

internal class DefaultQuery
{
    public static Expression<Func<T, bool>> True<T>() { return f => true; }
    public static Expression<Func<T, bool>> False<T>() { return f => false; }
}
