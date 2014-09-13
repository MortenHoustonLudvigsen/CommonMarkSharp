using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMarkSharp
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, ItemInfo, TResult> selector)
        {
            var lastIndex = source.Count() - 1;
            return source.Select((item, index) => selector(item, index, new ItemInfo(index, index == 0, index == lastIndex)));
        }

        public class ItemInfo
        {
            public ItemInfo(int index, bool isFirst, bool isLast)
            {
                Index = index;
                IsFirst = isFirst;
                IsLast = isLast;
            }

            public int Index { get; private set; }
            public bool IsFirst { get; private set; }
            public bool IsLast { get; private set; }
        }
    }
}
