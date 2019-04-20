namespace JobShopScheduling
{
    using System;
    using System.Linq;
    using GeneticAlgorithm;
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
                Print(geneticAlgorithm.Population, geneticAlgorithm.TimeEvolving);
            };
            geneticAlgorithm.TaskExecutor = new ParallelTaskExecutor()
            {
                MinThreads = 1,
                MaxThreads = Environment.ProcessorCount / 2
            };
            geneticAlgorithm.Start();
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