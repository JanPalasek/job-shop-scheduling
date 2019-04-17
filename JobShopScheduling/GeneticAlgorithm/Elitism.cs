namespace JobShopScheduling.GeneticAlgorithm
{
    using System.Collections.Generic;
    using System.Linq;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Populations;
    using GeneticSharp.Domain.Randomizations;
    using GeneticSharp.Domain.Reinsertions;

    public class Elitism : ReinsertionBase
    {
        private readonly float elitistPercentage;

        public Elitism(float elitistPercentage) : base(true, true)
        {
            this.elitistPercentage = elitistPercentage;
        }

        protected override IList<IChromosome> PerformSelectChromosomes(IPopulation population,
            IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            int takeBestCount = (int)(parents.Count * elitistPercentage);
            var bestResultsFromParents = parents.OrderByDescending(x => x.Fitness).Take(takeBestCount);

            // randomly remove some off-springs
            for (int i = 0; i < takeBestCount; i++)
            {
                offspring.RemoveAt(RandomizationProvider.Current.GetInt(0, offspring.Count));
            }

            // add best parents
            foreach (var parent in bestResultsFromParents)
            {
                offspring.Add(parent);
            }

            return offspring;
        }
    }
}