namespace JobShopScheduling.GeneticAlgorithm
{
    using System.Collections.Generic;
    using System.Linq;
    using GeneticSharp.Domain;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Crossovers;
    using GeneticSharp.Domain.Mutations;
    using GeneticSharp.Domain.Populations;
    using GeneticSharp.Domain.Randomizations;

    public class JobShopOperatorStrategy : IOperatorsStrategy
    {
        public IList<IChromosome> Cross(IPopulation population, ICrossover crossover, float crossoverProbability, IList<IChromosome> parents)
        {
            var offspring = new List<IChromosome>();

            for (int i = 0; i < population.MinSize; i += crossover.ParentsNumber)
            {
                var selectedParents = parents.Skip(i).Take(crossover.ParentsNumber).ToList();

                // If match the probability cross is made, otherwise the offspring is an exact copy of the parents.
                // Checks if the number of selected parents is equal which the crossover expect, because the in the end of the list we can
                // have some rest chromosomes.
                if (selectedParents.Count == crossover.ParentsNumber)
                {
                    offspring.AddRange(crossover.Cross(selectedParents));
                }
                else
                {
                    offspring.AddRange(selectedParents);

                    int leftToAdd = crossover.ParentsNumber - selectedParents.Count;
                    for (int j = 0; j < leftToAdd; j++)
                    {
                        offspring.Add(parents[RandomizationProvider.Current.GetInt(0, parents.Count)]);
                    }
                }
            }

            return offspring;
        }

        public void Mutate(IMutation mutation, float mutationProbability, IList<IChromosome> chromosomes)
        {
            foreach (var c in chromosomes)
            {
                mutation.Mutate(c, mutationProbability);
            }
        }
    }
}