namespace JobShopScheduling.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Advanced.Algorithms.DataStructures.Graph.AdjacencyList;
    using Advanced.Algorithms.Graph;
    using GeneticAlgorithm;
    using JobShopStructures;

    /// <summary>
    /// Component handling various operations on graphs.
    /// </summary>
    public class GraphHandler
    {
        /// <summary>
        /// Creates graph from specified chromosome.
        /// </summary>
        /// <param name="chromosome"></param>
        /// <returns></returns>
        public DiGraph<Operation> CreateGraph(ScheduleChromosome chromosome)
        {
            // dictionary mapping first operation to its successor
            var machineOperationsDictionary = new Dictionary<Operation, List<Operation>>();
            foreach (var machineChromosome in chromosome.GetGenes().Select(x => x.Value).Cast<MachineChromosome>().Where(x => x.Length >= 2))
            {
                var machineOperations = machineChromosome.GetGenes().Select(x => x.Value).Cast<Operation>().ToArray();
                for (int i = 0; i < machineOperations.Length; i++)
                {
                    var operations = new List<Operation>();
                    for (int j = i + 1; j < machineOperations.Length; j++)
                    {
                        // do not add operations on the same jobs (they have no reason)
                        if (machineOperations[j].JobId == machineOperations[i].JobId)
                        {
                            continue;
                        }
                        operations.Add(machineOperations[j]);
                    }
                    machineOperationsDictionary.Add(machineOperations[i], operations);
                }
            }

            var graph = new DiGraph<Operation>();

            var source = graph.AddVertex(new Operation(int.MinValue, int.MinValue, int.MinValue, int.MinValue, 0));
            var target = graph.AddVertex(new Operation(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, 0));
            foreach (var job in chromosome.JobShop.Jobs)
            {
                var lastOperationVertex = source;
                // connect one path
                foreach (var operation in job.Operations)
                {
                    var currentOperationVertex = graph.AddVertex(operation);
                    graph.AddEdge(lastOperationVertex.Value, currentOperationVertex.Value);

                    lastOperationVertex = currentOperationVertex;
                }

                // add edge from last on the path to target
                graph.AddEdge(lastOperationVertex.Value, target.Value);
            }

            // add machine edges
            foreach (var operation in chromosome.JobShop.Operations)
            {
                if (machineOperationsDictionary.TryGetValue(operation, out var nextMachineOperations))
                {
                    foreach (var nextMachineOperation in nextMachineOperations)
                    {
                        graph.AddEdge(operation, nextMachineOperation);
                    }
                }
            }

            return graph;
        }

        /// <summary>
        /// Breaks cycles, returning list of edges whose orientation was reversed during the process.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public List<(Operation Operation1, Operation Operation2)> BreakCycles(DiGraph<Operation> graph)
        {
            // break cycles
            var edgesThatChangedOrientation = new GraphCycleBreaker(
                Global.Config.BackEdgeSwitchOrientationProbability,
                Global.Config.ForwardEdgeSwitchOrientationProbability,
                Global.Config.SameLevelEdgeSwitchOrientationProbability)
                .BreakCycles(graph);

            return edgesThatChangedOrientation;
        }

        /// <summary>
        /// Detects, whether the specified <see cref="graph"/> has cycles.
        /// Returns true if it does, otherwise returns false.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public bool HasCycles(DiGraph<Operation> graph)
        {
            return new CycleDetector<Operation>().HasCycle(graph);
        }

        /// <summary>
        /// Obtains topological order of <see cref="Operation"/>s from <see cref="graph"/>.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public List<Operation> GetTopologicalOrder(DiGraph<Operation> graph)
        {
            return new DepthFirstTopSort<Operation>().GetTopSort(graph);
        }

        /// <summary>
        /// Computes cost of the most expensive path in the <see cref="graph"/>.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="topologicalOrder"></param>
        /// <returns></returns>
        public double GetMaximumCost(DiGraph<Operation> graph, IReadOnlyList<Operation> topologicalOrder)
        {
            double[] highestOperationCosts = new double[topologicalOrder.Count];

            // find longest distance to i-th operation for every i
            for (int i = 1; i < topologicalOrder.Count; i++)
            {
                // check all edges to the i-th vertex
                var operation = topologicalOrder[i];
                for (int j = 0; j < i; j++)
                {
                    var otherOperation = topologicalOrder[j];

                    if (graph.HasEdge(otherOperation, operation))
                    {
                        highestOperationCosts[i] = Math.Max(highestOperationCosts[i],
                            highestOperationCosts[j] + otherOperation.Cost);
                    }
                }
            }

            return highestOperationCosts.Last();
        }

        /// <summary>
        /// Computes maximum cost in <see cref="graph"/> using topological sort.
        /// </summary>
        /// <param name="graph">Graph from which the maximum cost will be calculated. It needs to be acyclic.</param>
        /// <returns></returns>
        public double GetMaximumCost(DiGraph<Operation> graph)
        {
            // topologically sort the graph => returns topologically sorted operations
            var topologicallySortedOperations = new DepthFirstTopSort<Operation>().GetTopSort(graph);

            return GetMaximumCost(graph, topologicallySortedOperations);
        }
    }
}