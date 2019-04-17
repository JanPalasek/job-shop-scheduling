namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Fitnesses;
    using GeneticSharp.Domain.Populations;
    using GeneticSharp.Domain.Randomizations;
    using GeneticSharp.Domain.Selections;

    public class JobShopTournamentSelection : ISelection
    {
        private readonly IFitness fitness;
        private readonly float probability;

        public JobShopTournamentSelection(IFitness fitness, float probability)
        {
            this.fitness = fitness;
            this.probability = probability;
        }

        public IList<IChromosome> SelectChromosomes(int number, Generation generation)
        {
            var result = new List<IChromosome>();

            var chromosomes = generation.Chromosomes;
            for (int i = 0; i < number; i++)
            {
                var chromosome1 = chromosomes[RandomizationProvider.Current.GetInt(0, chromosomes.Count)];
                var chromosome2 = chromosomes[RandomizationProvider.Current.GetInt(0, chromosomes.Count)];

                if (fitness.Evaluate(chromosome1) > fitness.Evaluate(chromosome2))
                {
                    result.Add(RandomizationProvider.Current.GetDouble() < probability ? chromosome1 : chromosome2);
                }
                else
                {
                    result.Add(RandomizationProvider.Current.GetDouble() < probability ? chromosome2 : chromosome1);
                }
            }

            return result;
        }
    }
}