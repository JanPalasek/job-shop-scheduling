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

        public static IEnumerable<int> RandomSequence(int minValue, int maxValue)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            while (true)
            {
                yield return random.Next(minValue, maxValue);
            }
        }
    }
}