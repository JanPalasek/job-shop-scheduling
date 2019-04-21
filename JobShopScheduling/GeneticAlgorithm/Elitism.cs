namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Populations;
    using GeneticSharp.Domain.Randomizations;
    using GeneticSharp.Domain.Reinsertions;

    /// <summary>
    /// This component performs the elitism.
    /// It copies <see cref="elitistPercentage"/> chromosomes from previous generation
    /// into the offspring.
    /// </summary>
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
            int takeBestCount = (int)(population.MinSize * elitistPercentage);
            var bestResults = population.CurrentGeneration.Chromosomes.OrderByDescending(x => x.Fitness).Take(takeBestCount);

            // randomly remove some off-springs (if adding would exceed the population)
            for (int i = 0; i < Math.Max(takeBestCount, offspring.Count - population.MaxSize); i++)
            {
                offspring.RemoveAt(RandomizationProvider.Current.GetInt(0, offspring.Count));
            }

            // add best parents
            foreach (var chromosome in bestResults)
            {
                offspring.Add(chromosome);
            }

            return offspring;
        }
    }
}