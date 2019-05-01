using System.Diagnostics;
using GeneticSharp.Domain.Mutations;

namespace JobShopScheduling
{
    using System;
    using System.Linq;
    using GeneticAlgorithm;
    using GeneticSharp.Domain.Crossovers;
    using GeneticSharp.Domain.Populations;
    using GeneticSharp.Domain.Selections;
    using GeneticSharp.Domain.Terminations;
    using GeneticSharp.Infrastructure.Framework.Threading;
    using JobShopStructures;
    using Utils;

    public class JobShopGeneticAlgorithm
    {
        private readonly JobShop jobShop;

        public JobShopGeneticAlgorithm(JobShop jobShop)
        {
            this.jobShop = jobShop;
        }

        public void Run()
        {
            var adamChromosome = new ScheduleChromosome(jobShop);
            var population = new Population(Config.MinPopulationSize, Config.MaxPopulationSize, adamChromosome);

            var fitness = new ScheduleFitness();
            var selection = new NonDeterministicTournamentSelection(Config.TournamentSelectionProbability);
            var crossover = new SchedulesCrossover(new CycleCrossover());
            var mutation = new ScheduleMutation(Config.InversionMutationPerGeneProbability, new ReverseSequenceMutation());
            var geneticAlgorithm =
                new GeneticSharp.Domain.GeneticAlgorithm(population, fitness, selection, crossover, mutation)
                {
                    Termination = new GenerationNumberTermination(Config.GenerationsCount),
                    MutationProbability = Config.MutationProbability,
                    CrossoverProbability = Config.CrossoverProbability,
                    OperatorsStrategy = new JobShopOperatorStrategy(),
                    Reinsertion = new JobShopReinsertion(
                        new EliteSelection(),
                        new Elitism(Config.ElitismPercent),
                        fitness
                    )
                };
            var stopWatch = new Stopwatch();
            geneticAlgorithm.GenerationRan += (sender, e) =>
            {
                Print(geneticAlgorithm.Population, stopWatch.Elapsed);
            };
            geneticAlgorithm.TaskExecutor = new ParallelTaskExecutor()
            {
                MinThreads = 1,
                MaxThreads = Environment.ProcessorCount / 2
            };
            stopWatch.Start();
            geneticAlgorithm.Start();
            stopWatch.Stop();
        }

        private void Print(IPopulation population, TimeSpan totalTime)
        {
            var bestChromosome = (ScheduleChromosome)population.BestChromosome;
            Console.WriteLine($"Generation: {population.GenerationsNumber}");
            Console.WriteLine($"Best schedule length: {bestChromosome.ScheduleLength:F}");
            var chromosomes = population.CurrentGeneration.Chromosomes.Cast<ScheduleChromosome>().ToList();
            Console.WriteLine($"Average schedule length: {chromosomes.Average(x => x.ScheduleLength):F}");
            Console.WriteLine($"Population std deviation: {chromosomes.StandardDeviation(x => (decimal)x.ScheduleLength.Value):F}");
            Console.WriteLine($"Time evolving: {totalTime.TotalSeconds:F}");
            Console.WriteLine();
        }
    }
}