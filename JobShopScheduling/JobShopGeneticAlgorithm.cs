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

        public void Run(int iterationsCount, bool adaptive = true)
        {
            if (iterationsCount < 1)
            {
                return;
            }
            
            ScheduleChromosome bestChromosome = null;
            for (int i = 0; i < iterationsCount; i++)
            {
                var chromosome = RunOnce(adaptive);
                Console.WriteLine();

                if (bestChromosome == null)
                {
                    bestChromosome = chromosome;
                }
                else
                {
                    bestChromosome = chromosome.Fitness > bestChromosome.Fitness ? chromosome : bestChromosome;
                }
            }
            Console.WriteLine($"Best chromosome for all iterations: {bestChromosome.ScheduleLength}");
        }

        private ScheduleChromosome RunOnce(bool adaptive = true)
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
            if (adaptive)
            {
                geneticAlgorithm.GenerationRan += (sender, e) => AdaptMutationProbability(geneticAlgorithm);;
            }
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

            return (ScheduleChromosome)geneticAlgorithm.BestChromosome;
        }
        
        private void Print(IPopulation population, TimeSpan totalTime)
        {
            var bestChromosome = (ScheduleChromosome)population.BestChromosome;
            Console.Write($"Generation: {population.GenerationsNumber}, ");
            Console.Write($"Best schedule length: {bestChromosome.ScheduleLength:F}, ");
            var chromosomes = population.CurrentGeneration.Chromosomes.Cast<ScheduleChromosome>().ToList();
            Console.Write($"Average schedule length: {chromosomes.Average(x => x.ScheduleLength):F}, ");
            Console.Write($"Population std deviation: {chromosomes.StandardDeviation(x => (decimal)x.ScheduleLength.Value):F}, ");
            Console.Write($"Time evolving: {totalTime.TotalSeconds:F}");
            Console.WriteLine();
        }

        private void AdaptMutationProbability(GeneticSharp.Domain.GeneticAlgorithm geneticAlgorithm)
        {
            var lastGenerations = geneticAlgorithm.Population.Generations.Take(10).ToList();

            var bestChromosome = geneticAlgorithm.BestChromosome;

            if (lastGenerations.Count < 10)
            {
                return;
            }

            bool haveSameFitness = lastGenerations.All(
                x => Math.Abs(x.BestChromosome.Fitness.Value - bestChromosome.Fitness.Value) < float.Epsilon);

            // all chromosomes have same fitness => increase mutation rate
            if (haveSameFitness)
            {
                geneticAlgorithm.MutationProbability *= 1.01f;
                geneticAlgorithm.MutationProbability =
                    Math.Min(geneticAlgorithm.MutationProbability, Config.MaximumMutationProbability);
            }
            // otherwise decrease it if they don't have same fitness
            else
            {
                geneticAlgorithm.MutationProbability /= 1.15f;
                geneticAlgorithm.MutationProbability =
                    Math.Max(geneticAlgorithm.MutationProbability, Config.MinimumMutationProbability);
            }
        }
    }
}