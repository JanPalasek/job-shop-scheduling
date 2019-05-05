using System.Diagnostics;
using System.IO;
using GeneticSharp.Domain.Mutations;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

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
    using Serilog;
    using Utils;

    /// <summary>
    /// Component that handles genetic algorithm evaluation including plotting etc.
    /// </summary>
    public class JobShopGeneticAlgorithm
    {
        private readonly JobShop jobShop;
        private readonly int iterationsCount;
        private readonly ILogger logger;
        private readonly bool adaptive;

        /// <summary>
        ///  Plot model (if you want to pass it instead of letting the component create it itself
        /// </summary>
        public PlotModel PlotModel { get; set; }

        /// <summary>
        /// Event that will be invoked after every generation.
        /// </summary>
        public event Action<Generation> GenerationRan;


        /// <summary>
        /// Creates instance of <see cref="JobShopGeneticAlgorithm"/> from parameters.
        /// </summary>
        /// <param name="jobShop">Input.</param>
        /// <param name="iterationsCount">How many times will the GA be run repeatedly.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="adaptive">True, if the algorithm should be adaptive.</param>
        /// <exception cref="ArgumentException"></exception>
        public JobShopGeneticAlgorithm(JobShop jobShop, int iterationsCount, ILogger logger, bool adaptive = true)
        {
            if (iterationsCount < 1)
            {
                throw new ArgumentException();
            }
            
            this.jobShop = jobShop;
            this.iterationsCount = iterationsCount;
            this.logger = logger;
            this.adaptive = adaptive;
        }

        public void Run()
        {
            logger.Information($"GA started");
            logger.Information($"Adaptive: {adaptive}");

            ScheduleChromosome bestChromosome = null;
            for (int i = 0; i < iterationsCount; i++)
            {
                logger.Information($"Iteration: {i + 1}");
                ScheduleChromosome chromosome;
                if (PlotModel == null)
                {
                    chromosome = RunOnce();
                }
                else
                {
                    var lineSeries = new LineSeries();
                    PlotModel.Series.Add(lineSeries);
                    chromosome = RunOnce(lineSeries);
                }
                logger.Information(string.Empty);

                if (bestChromosome == null)
                {
                    bestChromosome = chromosome;
                }
                else
                {
                    bestChromosome = chromosome.Fitness > bestChromosome.Fitness ? chromosome : bestChromosome;
                }
            }

            logger.Information($"Best chromosome for all iterations: {bestChromosome.ScheduleLength}");
        }

        private ScheduleChromosome RunOnce(LineSeries lineSeries = null)
        {
            var adamChromosome = new ScheduleChromosome(jobShop);
            var population = new Population(Global.Config.MinPopulationSize, Global.Config.MaxPopulationSize, adamChromosome);

            var fitness = new ScheduleFitness();
            var selection = new NonDeterministicTournamentSelection(Global.Config.TournamentSelectionProbability);
            var crossover = new SchedulesCrossover(new CycleCrossover());
            var mutation = new ScheduleMutation(Global.Config.MutationPerGeneProbability, new ReverseSequenceMutation());
            var geneticAlgorithm =
                new GeneticSharp.Domain.GeneticAlgorithm(population, fitness, selection, crossover, mutation)
                {
                    Termination = new GenerationNumberTermination(Global.Config.GenerationsCount),
                    MutationProbability = Global.Config.MutationProbability,
                    CrossoverProbability = Global.Config.CrossoverProbability,
                    OperatorsStrategy = new JobShopOperatorStrategy(),
                    Reinsertion = new JobShopReinsertion(
                        new EliteSelection(),
                        new Elitism(Global.Config.ElitismPercent),
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
            
            // plot the model
            if (PlotModel != null)
            {
                geneticAlgorithm.GenerationRan += (sender, e) =>
                {
                    lineSeries.Points.Add(new DataPoint(geneticAlgorithm.Population.GenerationsNumber,
                        ((ScheduleChromosome)geneticAlgorithm.Population.BestChromosome).ScheduleLength.Value));
                };
            }

            geneticAlgorithm.GenerationRan += (o, e) => GenerationRan?.Invoke(geneticAlgorithm.Population.CurrentGeneration);
            geneticAlgorithm.TaskExecutor = new ParallelTaskExecutor()
            {
                MinThreads = 1,
                MaxThreads = Global.Config.ThreadsCount
            };
            stopWatch.Start();
            geneticAlgorithm.Start();
            stopWatch.Stop();

            return (ScheduleChromosome)geneticAlgorithm.BestChromosome;
        }
        
        private void Print(IPopulation population, TimeSpan totalTime)
        {
            var bestChromosome = (ScheduleChromosome)population.BestChromosome;
            logger.Information($"Generation: {population.GenerationsNumber}");
            logger.Information($"Best schedule length: {bestChromosome.ScheduleLength:F}");
            var chromosomes = population.CurrentGeneration.Chromosomes.Cast<ScheduleChromosome>().ToList();
            logger.Information($"Average schedule length: {chromosomes.Average(x => x.ScheduleLength):F}");
            logger.Information($"Population std deviation: {chromosomes.StandardDeviation(x => (decimal)x.ScheduleLength.Value):F}");
            logger.Information($"Time evolving: {totalTime.TotalSeconds:F}");
            logger.Information(string.Empty);
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
                    Math.Min(geneticAlgorithm.MutationProbability, Global.Config.MaximumMutationProbability);
            }
            // otherwise decrease it if they don't have same fitness
            else
            {
                geneticAlgorithm.MutationProbability /= 1.15f;
                geneticAlgorithm.MutationProbability =
                    Math.Max(geneticAlgorithm.MutationProbability, Global.Config.MinimumMutationProbability);
            }
        }
    }
}