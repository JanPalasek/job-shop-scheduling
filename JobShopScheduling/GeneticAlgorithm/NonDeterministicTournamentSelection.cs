namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Fitnesses;
    using GeneticSharp.Domain.Populations;
    using GeneticSharp.Domain.Randomizations;
    using GeneticSharp.Domain.Selections;

    public class NonDeterministicTournamentSelection : ISelection
    {
        private readonly float probability;

        public NonDeterministicTournamentSelection(float probability)
        {
            this.probability = probability;
        }

        public IList<IChromosome> SelectChromosomes(int number, Generation generation)
        {
            var result = new List<IChromosome>();

            var chromosomes = generation.Chromosomes;
            for (int i = 0; i < number; i++)
            {
                IChromosome chromosome;
                var chromosome1 = chromosomes[RandomizationProvider.Current.GetInt(0, chromosomes.Count)];
                var chromosome2 = chromosomes[RandomizationProvider.Current.GetInt(0, chromosomes.Count)];

                if (chromosome1.Fitness > chromosome2.Fitness)
                {
                    chromosome = RandomizationProvider.Current.GetDouble() <= probability ? chromosome1 : chromosome2;
                }
                else
                {
                    chromosome = RandomizationProvider.Current.GetDouble() <= probability ? chromosome2 : chromosome1;
                }

                result.Add(chromosome);
            }

            return result;
        }
    }
}