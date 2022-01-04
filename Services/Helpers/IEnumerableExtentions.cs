using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Helpers
{
    public static class IEnumerableExtentions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int batchSize)
        {
            if(batchSize <= 0)
            {
                throw new ArgumentException(nameof(batchSize));
            }

            while (source.Any())
            {
                yield return source.Take(batchSize);
                source = source.Skip(batchSize);
            }
        }
    }
}