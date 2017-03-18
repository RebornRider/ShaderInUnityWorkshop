using System.Collections.Generic;
using System.Linq;

public static class EnumerableUtil
{
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> attributes)
    {
        return attributes ?? Enumerable.Empty<T>();
    }
}