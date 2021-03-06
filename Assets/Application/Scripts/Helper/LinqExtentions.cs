﻿using System.Collections.Generic;
using System.Linq;

namespace Kanji.Helper
{
    public static class LinqExtentions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            while (source.Any())
            {
                yield return source.Take(chunkSize);
                source = source.Skip(chunkSize);
            }
        }
    }
}
