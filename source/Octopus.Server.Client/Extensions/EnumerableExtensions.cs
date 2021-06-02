using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Client.Extensions
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool inclusive)
        {
            foreach (T item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
                else
                {
                    if (inclusive) yield return item;

                    yield break;
                }
            }
        }
    }
}
