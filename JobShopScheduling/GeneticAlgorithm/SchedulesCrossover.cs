namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Crossovers;

    public class SchedulesCrossover : ICrossover
    {
        private readonly ICrossover permutationCrossover;

        public bool IsOrdered { get; }
        public int ParentsNumber { get; }
        public int ChildrenNumber { get; }
        public int MinChromosomeLength { get; }

        public SchedulesCrossover(ICrossover permutationCrossover)
        {
            this.permutationCrossover = permutationCrossover;
            IsOrdered = true;
            ParentsNumber = 2;
            ChildrenNumber = 2;
            MinChromosomeLength = 0;
        }

        public IList<IChromosome> Cross(IList<IChromosome> parents)
        {
            var scheduleParent1 = parents[0];
            var scheduleParent2 = parents[1];

            if (scheduleParent1.Length != scheduleParent2.Length)
            {
                throw new ArgumentException("Different length of parents, cannot be crossed.");
            }

            var scheduleChild1 = (ScheduleChromosome)scheduleParent1.Clone();
            var scheduleChild2 = (ScheduleChromosome)scheduleParent2.Clone();
            for (int i = 0; i < scheduleParent1.Length; i++)
            {
                var machine1 = (MachineChromosome)scheduleParent1.GetGene(i).Value;
                var machine2 = (MachineChromosome)scheduleParent2.GetGene(i).Value;

                if (machine1.RealLength < 3)
                {
                    continue;
                }

                var result = permutationCrossover.Cross(new List<IChromosome>() { machine1, machine2 });

                var child1 = (MachineChromosome)result[0];
                var child2 = (MachineChromosome)result[1];

                // replace genes (automatically resets fitness)
                scheduleChild1.ReplaceGene(i, new Gene(child1));
                scheduleChild1.ScheduleLength = 0;
                scheduleChild2.ReplaceGene(i, new Gene(child2));
                scheduleChild2.ScheduleLength = 0;
            }

            return new List<IChromosome>() { scheduleChild1, scheduleChild2 };
        }
    }
}