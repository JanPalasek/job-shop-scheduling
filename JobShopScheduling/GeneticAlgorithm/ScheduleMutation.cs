namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using Advanced.Algorithms.Graph;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Mutations;
    using GeneticSharp.Domain.Randomizations;

    /// <summary>
    /// Performs mutation on the schedule individual.
    /// </summary>
    public class ScheduleMutation : MutationBase
    {
        private readonly float machineMutationProbability;
        private readonly IMutation machineMutation;

        public ScheduleMutation(float machineMutationProbability, IMutation machineMutation)
        {
            this.machineMutationProbability = machineMutationProbability;
            this.machineMutation = machineMutation;
        }

        /// <summary>
        /// With probability <see cref="probability"/> it mutates the individual.
        /// The mutation swaps two genes of machine chromosome with probability <see cref="machineMutationProbability"/>.
        /// This procedure is iterated as many times as there is machine chromosome genes.
        /// </summary>
        /// <param name="chromosome"></param>
        /// <param name="probability"></param>
        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            PerformMutate((ScheduleChromosome)chromosome, probability);
        }

        private void PerformMutate(ScheduleChromosome chromosome, float probability)
        {
            if (RandomizationProvider.Current.GetDouble() <= probability)
            {
                // perform mutation by machines
                foreach (var gene in chromosome.GetGenes())
                {
                    if (RandomizationProvider.Current.GetDouble() <= machineMutationProbability)
                    {
                        // perform swap (maybe more times)
                        machineMutation.Mutate((MachineChromosome) gene.Value, 1);
                        chromosome.ScheduleLength = null;
                        chromosome.Fitness = null;
                    }
                }
            }
        }
    }
}