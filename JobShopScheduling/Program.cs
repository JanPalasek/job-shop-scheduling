namespace JobShopScheduling
{
    using System;
    using Advanced.Algorithms.Graph;
    using GeneticAlgorithm;
    using GeneticSharp.Domain.Populations;
    using GeneticSharp.Domain.Selections;
    using GeneticSharp.Domain.Terminations;

    internal class Program
    {
        private static void Main(string[] args)
        {
            int populationSize = 150;

            var generator = new JobShopGenerator();
            int[] jobOperationCounts = {
                20, 10, 15
            };
            int machinesCount = 5;

            var jobShop = generator.Generate(jobOperationCounts, machinesCount);

            var adamChromosome = new ScheduleChromosome(jobShop);

            var population = new Population(populationSize, populationSize, adamChromosome);
            var fitness = new ScheduleFitness();
            var selection = new TournamentSelection();
            var crossover = new SchedulesCrossover();
            var mutation = new ScheduleMutation();
            var geneticAlgorithm =
                new GeneticSharp.Domain.GeneticAlgorithm(population, fitness, selection, crossover, mutation)
                {
                    Termination = new GenerationNumberTermination(50),
                    MutationProbability = 0.6f,
                    CrossoverProbability = 0.5f
                };
            geneticAlgorithm.GenerationRan += (sender, e) =>
            {
                var scheduleLength = ((ScheduleChromosome)geneticAlgorithm.BestChromosome).ScheduleLength;
                Console.WriteLine($"Generation: {geneticAlgorithm.GenerationsNumber} - Schedule length: {scheduleLength:F}");
            };
            geneticAlgorithm.Start();
        }
    }
}
