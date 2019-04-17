namespace JobShopScheduling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GeneticSharp.Domain.Randomizations;

    public static class EnumerableExtensions
    {
        public static IEnumerable<TType> AsShuffledEnumerable<TType>(this IEnumerable<TType> enumerable)
        {
            return enumerable.OrderBy(x => RandomizationProvider.Current.GetInt(0, int.MaxValue));
        }
    }
}