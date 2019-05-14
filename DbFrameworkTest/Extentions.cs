using System.Data;
using System;

namespace DbFrameworkTest
{
    public static class Extentions
    {
        public static void AddRange(this IDataParameterCollection col, Array args)
        {
            foreach (var item in args)
            {
                col.Add(item);
            }
        }
    }
}
