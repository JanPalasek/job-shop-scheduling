namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Crossovers;

    public class SchedulesCrossover : CrossoverBase
    {
        private readonly PartiallyMappedCrossover crossover;

        public SchedulesCrossover() : base(2, 2)
        {
            crossover = new PartiallyMappedCrossover();
        }

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            var typedParents = (IList<ScheduleChromosome>)parents;

            var scheduleParent1 = typedParents[0];
            var scheduleParent2 = typedParents[1];

            if (scheduleParent1.Length != scheduleParent2.Length)
            {
                throw new ArgumentException("Different length of parents, cannot be crossed.");
            }

            var scheduleChild1 = (ScheduleChromosome)scheduleParent1.CreateNew();
            var scheduleChild2 = (ScheduleChromosome)scheduleParent2.CreateNew();
            for (int i = 0; i < scheduleParent1.Length; i++)
            {
                var machine1 = (IChromosome)scheduleParent1.GetGene(i).Value;
                var machine2 = (IChromosome)scheduleParent2.GetGene(i).Value;

                var result = crossover.Cross(new List<IChromosome>() { machine1, machine2 });

                var child1 = (MachineChromosome)result[0];
                var child2 = (MachineChromosome)result[1];

                scheduleChild1.ReplaceGene(i, new Gene(child1));
                scheduleChild2.ReplaceGene(i, new Gene(child2));
            }

            return new List<IChromosome>() { scheduleChild1, scheduleChild2 };
        }
    }
}