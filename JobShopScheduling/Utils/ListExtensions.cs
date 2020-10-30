using System;
using System.Collections.Generic;

namespace JobShopScheduling.Utils
{
    public static class ListExtensions
    {
        /// <summary>
        /// Returns indices of list items that fulfil specified predicate.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public static IEnumerable<int> IndicesOf<TType>(this IList<TType> list, Func<TType, bool> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    yield return i;
                }
            }
        }
    }
}