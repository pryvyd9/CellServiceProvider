using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace DbFramework
{
    internal static class Extensions
    {
        /// <summary>
        /// Determines whether sequence contains exactly one element.
        /// </summary>
        internal static bool Only<T>(this IEnumerable<T> ts)
        {
            return ts.Count() == 1;
        }

        /// <summary>
        /// Determines whether sequence contains exactly one element.
        /// </summary>
        internal static bool Only<T>(this IEnumerable<T> ts, Func<T, bool> predicate)
        {
            return ts.Count() == 1 && predicate(ts.Single());
        }

        ///// <summary>
        ///// Determines whether sequence contains exactly one element.
        ///// </summary>
        //internal static bool None<T>(this IEnumerable<T> ts)
        //{
        //    return !ts.Any();
        //}
        internal static StringBuilder AppendJoin(this StringBuilder builder, string delimiter, IEnumerable<object> collection)
        {
            return builder.Append(string.Join(delimiter, collection));
        }

    }

}
