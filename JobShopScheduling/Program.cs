namespace JobShopScheduling
{
    using System;
    using System.Linq;
    using System.Reflection;
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
            //var generator = new JobShopGenerator();

            //var random = new Random(42);
            //((IRandomInjectable)generator).InjectRandom(random);

            //JobShop jobShop = generator.Generate(Config.OperationCounts, Config.MachinesCount);

            JobShop jobShop = new JobShopLoader().Load("Examples/la19.in");

            var adamChromosome = new ScheduleChromosome(jobShop);
            var population = new Population(Config.MinPopulationSize, Config.MaxPopulationSize, adamChromosome);

            var fitness = new ScheduleFitness();
            var selection = new NonDeterministicTournamentSelection(Config.TournamentSelectionProbability);
            var crossover = new SchedulesCrossover();
            var mutation = new ScheduleMutation(Config.MutationPerBitProbability);
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
            //ScheduleChromosome bestChromosome = null;
            geneticAlgorithm.GenerationRan += (sender, e) =>
            {
                Print(geneticAlgorithm);

                //var chromosome = (ScheduleChromosome)geneticAlgorithm.BestChromosome;

                //if (bestChromosome == null)
                //{
                //    bestChromosome = chromosome;
                //}
                //else if (chromosome.ScheduleLength > bestChromosome.ScheduleLength)
                //{
                //    Console.WriteLine("INCREASE IN BEST VALUE");
                //}

                //bestChromosome = chromosome;
            };
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
            var chromosomes = geneticAlgorithm.Population.CurrentGeneration.Chromosomes.Cast<ScheduleChromosome>();
            Console.WriteLine($"Average schedule length: {chromosomes.Average(x => x.ScheduleLength):F}");
            //Console.WriteLine($"Population variance: {chromosomes.Variance(x => (decimal)x.ScheduleLength.Value):F}");
            Console.WriteLine($"Population size: {geneticAlgorithm.Population.CurrentGeneration.Chromosomes.Count}");

            //foreach (var chromosome in geneticAlgorithm.Population.CurrentGeneration.Chromosomes
            //    .OrderBy(x => x.Fitness).Cast<ScheduleChromosome>())
            //{
            //    Console.WriteLine($"{chromosome.ScheduleLength:F}");
            //    Console.WriteLine(chromosome.GetOperationStrings());
            //}

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
