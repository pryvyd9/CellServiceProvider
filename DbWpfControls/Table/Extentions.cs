using System;
using System.Linq;
using System.Collections.Generic;

namespace DbWpfControls
{
    public static class Extentions
    {
        public static IEnumerable<(T,V)> Zip<T,V>(this IEnumerable<T> a, IEnumerable<V> b)
        {
            if (a.Count() != b.Count())
            {
                throw new Exception("enumerations' counts do not match.");
            }

            var e1 = a.GetEnumerator();
            var e2 = b.GetEnumerator();

            while(e1.MoveNext() && e2.MoveNext())
            {
                yield return (e1.Current, e2.Current);
            }
        }

        //public static IEnumerable<T> Unite<T>(this IEnumerable<T> a, IEnumerable<T> b)
        //{
        //    return a.Concat(b).Distinct();
        //}
        //public static IDictionary<T,V> ToDictionary<T,V>(this IEnumerable<(T,V)> col)
        //{
        //    return col.ToDictionary(n => n.Item1, n => n.Item2);
        //}
    }

}
