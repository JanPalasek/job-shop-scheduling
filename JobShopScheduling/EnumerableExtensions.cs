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

        public static decimal Variance<TType>(this IEnumerable<TType> enumerable, Func<TType, decimal> func)
        {
            enumerable = enumerable.ToList();

            decimal avg = enumerable.Average(func);

            decimal sumValue = 0;
            foreach (decimal value in enumerable.Select(func))
            {
                sumValue += (value - avg) * (value - avg);
            }

            decimal variance = sumValue / enumerable.Count();

            return variance;
        }
    }
}