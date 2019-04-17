namespace JobShopScheduling
{
    using System;
    using System.Linq;
    using Advanced.Algorithms.Graph;
    using GeneticAlgorithm;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Fitnesses;
    using GeneticSharp.Domain.Populations;
    using GeneticSharp.Domain.Randomizations;
    using GeneticSharp.Domain.Selections;
    using GeneticSharp.Domain.Terminations;
    using GeneticSharp.Infrastructure.Framework.Threading;

    internal class Program
    {
        private static void Main(string[] args)
        {
            int populationSize = 200;

            var generator = new JobShopGenerator();
            int[] jobOperationCounts = {
                30, 35, 29, 30, 30, 20, 30
            };
            int machinesCount = 15;

            var jobShop = generator.Generate(jobOperationCounts, machinesCount);

            var adamChromosome = new ScheduleChromosome(jobShop);

            var population = new Population(populationSize, populationSize, adamChromosome);

            var fitness = new ScheduleFitness();
            var selection = new JobShopTournamentSelection(fitness, Config.TournamentProbability);
            var crossover = new SchedulesCrossover();
            var mutation = new ScheduleMutation(Config.MutationPerBitProbability);
            var geneticAlgorithm =
                new GeneticSharp.Domain.GeneticAlgorithm(population, fitness, selection, crossover, mutation)
                {
                    Termination = new GenerationNumberTermination(100),
                    MutationProbability = Config.MutationProbability,
                    CrossoverProbability = Config.CrossoverProbability,
                    OperatorsStrategy = new JobShopOperatorStrategy(),
                    Reinsertion = new Elitism(Config.ElitismPercent)
                };
            geneticAlgorithm.GenerationRan += (sender, e) => { Print(geneticAlgorithm); };
            //geneticAlgorithm.TaskExecutor = new ParallelTaskExecutor()
            //{
            //    MinThreads = 1,
            //    MaxThreads = 4
            //};
            geneticAlgorithm.Start();
        }

        private static void Print(GeneticSharp.Domain.GeneticAlgorithm geneticAlgorithm)
        {
            var bestChromosome = (ScheduleChromosome)geneticAlgorithm.BestChromosome;
            Console.WriteLine($"Generation: {geneticAlgorithm.GenerationsNumber}");
            Console.WriteLine($"Best schedule length: {bestChromosome.ScheduleLength:F}");
            var avg = geneticAlgorithm.Population.CurrentGeneration.Chromosomes.Cast<ScheduleChromosome>()
                .Average(x => x.ScheduleLength);
            Console.WriteLine($"Average schedule length: {avg:F}");
            Console.WriteLine($"Population size: {geneticAlgorithm.Population.CurrentGeneration.Chromosomes.Count}");

            //Console.WriteLine(bestChromosome.GetOperationStrings());
        }
    }
}
