namespace JobShopScheduling.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GeneticSharp.Domain.Randomizations;

    public static class EnumerableExtensions
    {
        /// <summary>
        /// Shuffles randomly input using <see cref="RandomizationProvider"/>.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static IEnumerable<TType> AsShuffledEnumerable<TType>(this IEnumerable<TType> enumerable)
        {
            return enumerable.OrderBy(x => RandomizationProvider.Current.GetInt(0, int.MaxValue));
        }

        /// <summary>
        /// Computes standard deviation from the <see cref="enumerable"/> parameter.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="func"></param>
        /// <returns>Standard deviation.</returns>
        public static double StandardDeviation<TType>(this IEnumerable<TType> enumerable, Func<TType, decimal> func)
        {
            enumerable = enumerable.ToList();

            decimal avg = enumerable.Average(func);

            decimal sumValue = 0;
            foreach (decimal value in enumerable.Select(func))
            {
                sumValue += (value - avg) * (value - avg);
            }

            decimal variance = sumValue / enumerable.Count();

            return Math.Sqrt((double)variance);
        }
    }
}