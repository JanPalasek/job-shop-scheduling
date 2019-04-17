namespace JobShopScheduling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Advanced.Algorithms.DataStructures.Graph.AdjacencyList;
    using Advanced.Algorithms.Graph;
    using GeneticSharp.Domain.Randomizations;

    /// <summary>
    /// Component that is designed to break cycles of the graph. It changes orientation of edges so
    /// there are not oriented cycles anymore.
    /// </summary>
    public class CycleBreaker
    {
        private readonly double backEdgeBreakProbability;
        private readonly double otherEdgeBreakProbability;

        public CycleBreaker(double backEdgeBreakProbability, double otherEdgeBreakProbability)
        {
            this.backEdgeBreakProbability = backEdgeBreakProbability;
            this.otherEdgeBreakProbability = otherEdgeBreakProbability;
        }

        /// <summary>
        /// Breaks cycles in the graph, returning all edges that changed their orientation.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public List<(Operation Operation1, Operation Operation2)> BreakCycles(DiGraph<Operation> graph)
        {
            var changedOrientationEdges = new List<(Operation Operation1, Operation Operation2)>();

            var algorithm = new TarjansStronglyConnected<Operation>();

            List<List<Operation>> components;
            do
            {
                // find strongly connected components
                components = algorithm.FindStronglyConnectedComponents(graph).Where(x => x.Count > 1).ToList();
                foreach (List<Operation> component in components)
                {
                    var componentHashSet = component.ToHashSet();
                    foreach (var operation1 in component.AsShuffledEnumerable())
                    {
                        // get all neighbor operations that are in the same component
                        var neighborOperations = GetNeighborComponentOperations(graph, operation1, componentHashSet);

                        foreach (var operation2 in neighborOperations)
                        {
                            // two operations are not on the same job and edge is between them
                            if (operation1.JobId != operation2.JobId
                                && operation1.MachineId == operation2.MachineId)
                            {
                                // if it is back edge => switch orientation of the edge with backEdgeBreakProbability
                                if (operation1.Order > operation2.Order
                                    && RandomizationProvider.Current.GetDouble() < backEdgeBreakProbability)
                                {
                                    // switch edge orientation
                                    graph.RemoveEdge(operation1, operation2);
                                    graph.AddEdge(operation2, operation1);

                                    changedOrientationEdges.Add((operation1, operation2));
                                    goto cycleOut;
                                }

                                // if it isn't back edge => switch orientation of the edge with otherEdgeBreakProbability
                                if (operation1.Order <= operation2.Order
                                         && RandomizationProvider.Current.GetDouble() < otherEdgeBreakProbability)
                                {
                                    // switch edge orientation
                                    graph.RemoveEdge(operation1, operation2);
                                    graph.AddEdge(operation2, operation1);

                                    changedOrientationEdges.Add((operation1, operation2));
                                    goto cycleOut;
                                }
                            }
                        }
                    }
                }

            cycleOut:;
            } while (components.Count > 0);

            return changedOrientationEdges;
        }

        private IEnumerable<Operation> GetNeighborComponentOperations(DiGraph<Operation> graph,
            Operation operation1, HashSet<Operation> componentOperations)
        {
            var vertex = graph.FindVertex(operation1);

            foreach (var vertexOutEdge in vertex.OutEdges)
            {
                if (componentOperations.Contains(vertexOutEdge.Value))
                {
                    yield return vertexOutEdge.Value;
                }
            }
        }
    }
}