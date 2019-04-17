namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using Advanced.Algorithms.Graph;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Mutations;

    public class ScheduleMutation : MutationBase
    {
        private readonly TworsMutation swapMutation;
        private readonly Random random;

        public ScheduleMutation()
        {
            this.swapMutation = new TworsMutation();
            random = new Random();
        }

        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            PerformMutate((ScheduleChromosome) chromosome, probability);
        }

        private void PerformMutate(ScheduleChromosome chromosome, float probability)
        {
            // perform mutation by machines
            foreach (var gene in chromosome.GetGenes())
            {
                bool mutate = random.NextDouble() < probability;
                if (mutate)
                {
                    // perform swap (maybe more times)
                    swapMutation.Mutate((IChromosome)gene.Value, 1);
                    chromosome.ScheduleLength = null;
                }
            }
        }
    }
}