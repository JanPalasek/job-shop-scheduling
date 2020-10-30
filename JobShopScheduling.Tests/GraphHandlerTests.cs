namespace JobShopScheduling.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Advanced.Algorithms.DataStructures.Graph.AdjacencyList;
    using Advanced.Algorithms.Graph;
    using GeneticAlgorithm;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Randomizations;
    using Graph;
    using JobShopStructures;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class GraphHandlerTests
    {
        private ScheduleChromosome chromosome;
        private JobShop jobShop;
        private GraphHandler graphHandler;

        [SetUp]
        public void SetUp()
        {
            jobShop = new JobShopLoader().Load("TestExamples/test1.in", false);
            chromosome = new ScheduleChromosome(jobShop);

            var machineChromosome1 = new MachineChromosome(new int[]
            {
                4, 5, 0, 2
            }.Select(x => jobShop.Operations[x]).ToArray());
            chromosome.ReplaceGene(0, new Gene(machineChromosome1));

            var machineChromosome2 = new MachineChromosome(new int[]
            {
                1, 3
            }.Select(x => jobShop.Operations[x]).ToArray());
            chromosome.ReplaceGene(1, new Gene(machineChromosome2));

            graphHandler = new GraphHandler();
        }

        [Test]
        public void CreateGraphAndHasCycleTest()
        {
            var graph = graphHandler.CreateGraph(chromosome);

            // reference vertex is source
            var source = graph.ReferenceVertex;
            Assert.That(source.InEdges, Is.Empty);
            Assert.That(source.OutEdges, Has.Count.EqualTo(2));

            var target = graph.FindVertex(new Operation(int.MaxValue, 0, 0, 0, 0));
            Assert.That(target.InEdges, Has.Count.EqualTo(2));
            Assert.That(target.OutEdges, Is.Empty);

            var operation0 = graph.FindVertex(jobShop.Operations[0]);
            var operation1 = graph.FindVertex(jobShop.Operations[1]);
            var operation2 = graph.FindVertex(jobShop.Operations[2]);
            var operation3 = graph.FindVertex(jobShop.Operations[3]);
            var operation4 = graph.FindVertex(jobShop.Operations[4]);
            var operation5 = graph.FindVertex(jobShop.Operations[5]);

            // 0-th operation
            Assert.That(operation0.InEdges.Count, Is.EqualTo(3));
            Assert.That(operation0.InEdges, Does.Contain(source));
            Assert.That(operation0.InEdges, Does.Contain(operation4));
            Assert.That(operation0.InEdges, Does.Contain(operation5));
            Assert.That(operation0.OutEdges.Count, Is.EqualTo(1));
            Assert.That(operation0.OutEdges, Does.Contain(operation1));

            // 1-th operation
            Assert.That(operation1.InEdges.Count, Is.EqualTo(1));
            Assert.That(operation1.InEdges, Does.Contain(operation0));
            Assert.That(operation1.OutEdges.Count, Is.EqualTo(2));
            Assert.That(operation1.OutEdges, Does.Contain(operation2));
            Assert.That(operation1.OutEdges, Does.Contain(operation3));

            // 2-th operation
            Assert.That(operation2.InEdges.Count, Is.EqualTo(3));
            Assert.That(operation2.InEdges, Does.Contain(operation1));
            Assert.That(operation2.InEdges, Does.Contain(operation5));
            Assert.That(operation2.InEdges, Does.Contain(operation4));
            Assert.That(operation2.OutEdges.Count, Is.EqualTo(1));
            Assert.That(operation2.OutEdges, Does.Contain(target));

            // 3-th operation
            Assert.That(operation3.InEdges.Count, Is.EqualTo(2));
            Assert.That(operation3.InEdges, Does.Contain(source));
            Assert.That(operation3.InEdges, Does.Contain(operation1));
            Assert.That(operation3.OutEdges.Count, Is.EqualTo(1));
            Assert.That(operation3.OutEdges, Does.Contain(operation4));

            // 4-th operation
            Assert.That(operation4.InEdges.Count, Is.EqualTo(1));
            Assert.That(operation4.InEdges, Does.Contain(operation3));
            Assert.That(operation4.OutEdges.Count, Is.EqualTo(3));
            Assert.That(operation4.OutEdges, Does.Contain(operation5));
            Assert.That(operation4.OutEdges, Does.Contain(operation2));
            Assert.That(operation4.OutEdges, Does.Contain(operation0));

            // 5-th operation
            Assert.That(operation5.InEdges.Count, Is.EqualTo(1));
            Assert.That(operation5.InEdges, Does.Contain(operation4));
            Assert.That(operation5.OutEdges.Count, Is.EqualTo(3));
            Assert.That(operation5.OutEdges, Does.Contain(target));
            Assert.That(operation5.OutEdges, Does.Contain(operation0));
            Assert.That(operation5.OutEdges, Does.Contain(operation2));

            // has cycle verification
            Assert.That(graphHandler.HasCycles(graph), Is.True);
        }

        [Test(Description = "Test removing back edges.")]
        public void BreakCyclesReverseBackEdgeTest()
        {
            var randomizationProviderMock = new Mock<IRandomization>();
            randomizationProviderMock.Setup(x => x.GetDouble()).Returns(Global.Config.BackEdgeSwitchOrientationProbability - 10e-6);
            // fix random sort order of AsShuffledEnumerable
            randomizationProviderMock.Setup(x => x.GetInt(0, int.MaxValue)).Returns(0);
            RandomizationProvider.Current = randomizationProviderMock.Object;

            var graph = graphHandler.CreateGraph(chromosome);

            bool[,] edgesBefore = GetNeighborhoodMatrix(graph, jobShop);

            List<(Operation Operation1, Operation Operation2)> reversedEdges = graphHandler.BreakCycles(graph);

            Assert.That(graphHandler.HasCycles(graph), Is.False);
            Assert.That(reversedEdges, Has.Count.EqualTo(2));
            Assert.That(reversedEdges, Does.Contain((jobShop.Operations[5], jobShop.Operations[0])));
            Assert.That(reversedEdges, Does.Contain((jobShop.Operations[4], jobShop.Operations[0])));

            AssertGraphEqualExceptFor(edgesBefore, graph, reversedEdges);
        }

        [Test]
        public void GetMaximumCostTest()
        {
            var randomizationProviderMock = new Mock<IRandomization>();
            randomizationProviderMock.Setup(x => x.GetDouble()).Returns(Global.Config.BackEdgeSwitchOrientationProbability - 10e-6);
            // fix random sort order of AsShuffledEnumerable
            randomizationProviderMock.Setup(x => x.GetInt(0, int.MaxValue)).Returns(0);
            RandomizationProvider.Current = randomizationProviderMock.Object;

            var graph = graphHandler.CreateGraph(chromosome);
            graphHandler.BreakCycles(graph);

            double maximumCost = graphHandler.GetMaximumCost(graph);

            Assert.That(maximumCost, Is.EqualTo(21));
        }

        private void AssertGraphEqualExceptFor(bool[,] edgesBefore, DiGraph<Operation> newGraph, List<(Operation Operation1, Operation Operation2)> exceptForEdges)
        {
            // assert all edges
            foreach (var operation1 in jobShop.Operations)
            {
                foreach (var operation2 in jobShop.Operations.Where(x => x.Id != operation1.Id))
                {
                    // if the edge was reversed
                    if (exceptForEdges.Contains((operation1, operation2)))
                    {
                        Assert.That(newGraph.HasEdge(operation1, operation2), Is.False);
                    }
                    // if the edge has been there before => assert the edge is still there after removing the cycle
                    else if (edgesBefore[operation1.Id, operation2.Id])
                    {
                        Assert.That(newGraph.HasEdge(operation1, operation2), Is.True);
                    }
                }
            }
        }

        private bool[,] GetNeighborhoodMatrix(DiGraph<Operation> graph, JobShop jobShop)
        {
            bool[,] edges = new bool[jobShop.Operations.Length, jobShop.Operations.Length];
            for (int i = 0; i < jobShop.Operations.Length; i++)
            {
                for (int j = 0; j < jobShop.Operations.Length && i != j; j++)
                {
                    edges[i, j] = graph.HasEdge(jobShop.Operations[i], jobShop.Operations[j]);
                }
            }

            return edges;
        }
    }
}