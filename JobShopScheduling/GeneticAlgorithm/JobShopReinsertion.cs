using System.Threading.Tasks;

namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Fitnesses;
    using GeneticSharp.Domain.Populations;
    using GeneticSharp.Domain.Randomizations;
    using GeneticSharp.Domain.Reinsertions;
    using GeneticSharp.Domain.Selections;

    public class JobShopReinsertion : ReinsertionBase
    {
        private readonly ISelection selection;
        private readonly IReinsertion elitism;
        private readonly IFitness fitness;

        public JobShopReinsertion(ISelection selection, IReinsertion elitism, IFitness fitness) : base(true, true)
        {
            this.selection = selection;
            this.elitism = elitism;
            this.fitness = fitness;
        }

        protected override IList<IChromosome> PerformSelectChromosomes(IPopulation population,
            IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            // evaluate fitness of all
            Parallel.ForEach(offspring.Concat(parents),
                new ParallelOptions() { MaxDegreeOfParallelism = Global.Config.ThreadsCount },
                x => fitness.Evaluate(x));

            IList<IChromosome> resultOffspring;
            // if not enough offsprings => take from parents
            if (offspring.Count < population.MinSize)
            {
                var parentsToAdd = selection.SelectChromosomes(population.MinSize - offspring.Count,
                    new Generation(parents.Count, parents));
                resultOffspring = offspring;
                foreach (var chromosome in parentsToAdd)
                {
                    resultOffspring.Add(chromosome);
                }
            }
            // if too many offsprings => take right number from them
            else if (offspring.Count > population.MinSize)
            {
                resultOffspring = selection.SelectChromosomes(population.MinSize,
                    new Generation(offspring.Count, offspring));
            }
            else
            {
                resultOffspring = offspring;
            }
            // perform elitism (take n best samples from parents and add it to offsprings)
            resultOffspring = elitism.SelectChromosomes(population, resultOffspring, parents);

            return resultOffspring;
        }
    }
}