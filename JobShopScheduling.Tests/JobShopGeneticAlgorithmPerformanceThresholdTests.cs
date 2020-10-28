using System;
using System.Reflection;
using GeneticSharp.Domain.Randomizations;
using JobShopScheduling.JobShopStructures;
using Moq;
using NUnit.Framework;
using Serilog;

namespace JobShopScheduling.Tests
{
    /// <summary>
    /// Checks whether it can find the best result on easy tasks. This test is a lower performance threshold for the algorithm.
    /// </summary>
    [TestFixture]
    public class JobShopGeneticAlgorithmPerformanceThresholdTests
    {
        /// <summary>
        /// Runs performance threshold test.
        ///
        /// This configurations graph is depicted in test2.graphml graph file with arrows specifying order of machine evaluation.
        /// </summary>
        [Test]
        public void RunTest()
        {
            var jobShop = new JobShopLoader().Load("TestExamples/test2.in");
            var mockLogger = new Mock<ILogger>();
            
            Global.Config.MinPopulationSize = 100;
            Global.Config.MaxPopulationSize = 100;
            Global.Config.GenerationsCount = 10;
            Global.Config.IterationsCount = 1;
            Global.Config.CrossoverProbability = 0.75f;
            Global.Config.MutationProbability = 0.3f;
            Global.Config.MutationPerGeneProbability = 0.01f;
            Global.Config.ElitismPercent = 0.02f;
            Global.Config.ThreadsCount = 1;

            var ga = new JobShopGeneticAlgorithm(jobShop, Global.Config.IterationsCount, mockLogger.Object, adaptive: true);
            
            typeof(BasicRandomization).GetField("_globalRandom", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, new Random(42));
            RandomizationProvider.Current = new BasicRandomization();

            ga.Run();

            int expectedScheduleLength = 26;
            Assert.That(ga.BestSchedule.ScheduleLength, Is.EqualTo(expectedScheduleLength));
        }
    }
}