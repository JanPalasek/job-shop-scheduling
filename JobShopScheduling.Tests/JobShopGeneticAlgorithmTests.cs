namespace JobShopScheduling.Tests
{
    using System.Linq;
    using GeneticAlgorithm;
    using GeneticSharp.Domain;
    using GeneticSharp.Domain.Populations;
    using GeneticSharp.Domain.Randomizations;
    using JobShopStructures;
    using Moq;
    using NUnit.Framework;
    using Serilog;

    [TestFixture]
    public class JobShopGeneticAlgorithmTests
    {
        private JobShopGeneticAlgorithm ga;

        [SetUp]
        public void SetUp()
        {
            var jobShop = new JobShopLoader().Load("TestExamples/ft06.in");
            var mockLogger = new Mock<ILogger>();

            Global.Config.MinPopulationSize = 100;
            Global.Config.MaxPopulationSize = 100;
            ga = new JobShopGeneticAlgorithm(jobShop, 1, mockLogger.Object, adaptive: false);
            ga.GenerationRan += AssertUnique;
        }

        [Test]
        public void RunTest1()
        {
            Global.Config.GenerationsCount = 20;
            Global.Config.CrossoverProbability = 1f;
            Global.Config.MutationProbability = 1f;
            Global.Config.ElitismPercent = 0.5f;

            ga.Run();
        }
        [Test]
        public void RunTest2()
        {
            Global.Config.GenerationsCount = 20;
            Global.Config.CrossoverProbability = 0f;
            Global.Config.MutationProbability = 0f;
            Global.Config.ElitismPercent = 0.5f;

            ga.Run();
        }


        /// <summary>
        /// Asserts that all <see cref="ScheduleChromosome"/> in specified generation are represented by a different instance
        /// and all its genes are represented by different instances as well.
        ///
        /// If two members of generation would share same instance, they would be mutated potentially twice (mutation doesn't create
        /// a new instance) and thus breaking the GA.
        /// </summary>
        /// <param name="generation"></param>
        public void AssertUnique(Generation generation)
        {
            var chromosomes = generation.Chromosomes.Cast<ScheduleChromosome>().ToArray();
            for (int i = 0; i < chromosomes.Length; i++)
            {
                for (int j = 0; j < chromosomes.Length && j != i; j++)
                {
                    Assert.That(chromosomes[i], Is.Not.SameAs(chromosomes[j]));

                    var iMachines = chromosomes[i].GetGenes().Select(x => x.Value).ToList();
                    var jMachines = chromosomes[j].GetGenes().Select(x => x.Value).ToList();

                    // no machines in second chromosome is same as machine in the first chromosome
                    foreach (var iMachine in iMachines)
                    {
                        Assert.That(jMachines.Any(x => object.ReferenceEquals(x, iMachine)), Is.False);
                    }
                }
            }
        }
    }
}