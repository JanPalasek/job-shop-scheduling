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
        private readonly float mutationPerBitProbability;
        private readonly TworsMutation swapMutation;

        public ScheduleMutation(float mutationPerBitProbability)
        {
            this.mutationPerBitProbability = mutationPerBitProbability;
            this.swapMutation = new TworsMutation();
        }

        /// <summary>
        /// With probability <see cref="probability"/> it mutates the individual.
        /// The mutation swaps two genes of machine chromosome with probability <see cref="mutationPerBitProbability"/>.
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
                    var machineChromosome = (MachineChromosome)gene.Value;
                    for (int i = 0; i < machineChromosome.RealLength; i++)
                    {
                        if (RandomizationProvider.Current.GetDouble() <= mutationPerBitProbability)
                        {
                            // perform swap (maybe more times)
                            swapMutation.Mutate((MachineChromosome)gene.Value, 1);
                            chromosome.ScheduleLength = null;
                            chromosome.Fitness = null;
                        }
                    }
                }
            }
        }
    }
}