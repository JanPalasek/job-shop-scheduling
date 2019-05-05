using GeneticSharp.Domain.Selections;

namespace JobShopScheduling.Graph
{
    using System.Collections.Generic;
    using System.Linq;
    using Advanced.Algorithms.DataStructures.Graph.AdjacencyList;
    using Advanced.Algorithms.Graph;
    using GeneticSharp.Domain.Randomizations;
    using JobShopStructures;
    using Utils;

    /// <summary>
    /// Component that is designed to break cycles of the graph. It changes orientation of edges so
    /// there are not oriented cycles anymore.
    /// </summary>
    internal class GraphCycleBreaker
    {
        private readonly double backEdgeBreakProbability;
        private readonly double forwardEdgeBreakProbability;
        private readonly double sameLevelEdgeBreakProbability;

        public GraphCycleBreaker(double backEdgeBreakProbability, double forwardEdgeBreakProbability, double sameLevelEdgeBreakProbability)
        {
            this.backEdgeBreakProbability = backEdgeBreakProbability;
            this.forwardEdgeBreakProbability = forwardEdgeBreakProbability;
            this.sameLevelEdgeBreakProbability = sameLevelEdgeBreakProbability;
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
                foreach (List<Operation> component in components.AsShuffledEnumerable())
                {
                    var componentHashSet = component.ToHashSet();
                    foreach (var operation1 in component.AsShuffledEnumerable())
                    {
                        // get all neighbor operations that are in the same component
                        var neighborOperations = GetNeighborComponentOperations(graph, operation1, componentHashSet);

                        foreach (var operation2 in neighborOperations.AsShuffledEnumerable())
                        {
                            // two operations are not on the same job
                            // but are on the same machines (=> can switch their edge orientation if I need to)
                            if (operation1.JobId != operation2.JobId
                                && operation1.MachineId == operation2.MachineId)
                            {
                                double probability = RandomizationProvider.Current.GetDouble();
                                // switch directions of edge with prob for back edge, forward edge or same level edge
                                if ((operation1.Order > operation2.Order && probability <= backEdgeBreakProbability)
                                    || (operation1.Order < operation2.Order && probability <= forwardEdgeBreakProbability)
                                    || (operation1.Order == operation2.Order && probability <= sameLevelEdgeBreakProbability))
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