namespace JobShopScheduling
{
    using System;
    using System.Linq;
    using Advanced.Algorithms.Graph;
    using GeneticAlgorithm;
    using GeneticSharp.Domain.Populations;
    using GeneticSharp.Domain.Randomizations;
    using GeneticSharp.Domain.Selections;
    using GeneticSharp.Domain.Terminations;

    internal class Program
    {
        private static void Main(string[] args)
        {
            int populationSize = 500;

            var generator = new JobShopGenerator();
            int[] jobOperationCounts = {
                25, 20, 25, 20, 25, 20
            };
            int machinesCount = 10;

            var jobShop = generator.Generate(jobOperationCounts, machinesCount);

            var adamChromosome = new ScheduleChromosome(jobShop);

            var population = new Population(populationSize, populationSize, adamChromosome);
            var fitness = new ScheduleFitness();
            var selection = new TournamentSelection();
            var crossover = new SchedulesCrossover();
            var mutation = new ScheduleMutation(Config.MutationPerBitProbability);
            var geneticAlgorithm =
                new GeneticSharp.Domain.GeneticAlgorithm(population, fitness, selection, crossover, mutation)
                {
                    Termination = new GenerationNumberTermination(100),
                    MutationProbability = Config.MutationProbability,
                    CrossoverProbability = Config.CrossoverProbability,
                    //Reinsertion = new Elitism(Config.ElitismPercent)
                };
            geneticAlgorithm.GenerationRan += (sender, e) =>
            {
                var bestChromosome = (ScheduleChromosome) geneticAlgorithm.BestChromosome;
                fitness.Evaluate(bestChromosome);
                Console.WriteLine($"Generation: {geneticAlgorithm.GenerationsNumber}");
                Console.WriteLine($"Best schedule length: {bestChromosome.ScheduleLength:F}");
                var avg = geneticAlgorithm.Population.CurrentGeneration.Chromosomes.Cast<ScheduleChromosome>()
                    .Average(x => x.ScheduleLength);
                Console.WriteLine($"Average schedule length: {avg:F}");
                
                Console.WriteLine(bestChromosome.GetOperationStrings());
                
            };
            geneticAlgorithm.Start();
        }
    }
}
