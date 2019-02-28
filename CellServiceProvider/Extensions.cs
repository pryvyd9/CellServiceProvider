using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CellServiceProvider
{
    public static class Extensions
    {
        /// <summary>
        /// Determines whether sequence contains exactly one element.
        /// </summary>
        public static bool Only<T>(this IEnumerable<T> ts)
        {
            return ts.Count() == 1;
        }

        /// <summary>
        /// Determines whether sequence contains exactly one element.
        /// </summary>
        public static bool Only<T>(this IEnumerable<T> ts, Func<T, bool> predicate)
        {

            return ts.Count() == 1 && predicate(ts.Single());
        }

    }

}
