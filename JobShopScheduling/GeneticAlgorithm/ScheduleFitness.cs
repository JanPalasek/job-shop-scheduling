namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Fitnesses;

    public class ScheduleFitness : IFitness
    {
        public double Evaluate(IChromosome chromosome)
        {
            return Evaluate((ScheduleChromosome) chromosome);
        }

        private double Evaluate(ScheduleChromosome chromosome)
        {
            // TODO: calculate total length of the schedule
            throw new NotImplementedException();
        }
    }
}