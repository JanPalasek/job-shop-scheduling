namespace JobShopScheduling.GeneticAlgorithm
{
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Mutations;

    public class ScheduleMutation : MutationBase
    {
        private readonly TworsMutation swapMutation;

        public ScheduleMutation()
        {
            this.swapMutation = new TworsMutation();
        }

        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            // TODO: perform mutation by machines
            var scheduleChromosome = (ScheduleChromosome) chromosome;

            foreach (var gene in scheduleChromosome.GetGenes())
            {
                // perform swap (maybe more times)
                swapMutation.Mutate((IChromosome)gene.Value, probability);
            }

            // TODO: perform fix of the structure (after mutation and crossover the structure can be invalid => we need to fix it)
        }
    }
}