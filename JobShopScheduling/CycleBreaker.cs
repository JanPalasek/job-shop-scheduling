namespace JobShopScheduling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Advanced.Algorithms.DataStructures.Graph.AdjacencyList;
    using Advanced.Algorithms.Graph;

    public class CycleBreaker
    {
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
                components = algorithm.FindStronglyConnectedComponents(graph).Where(x => x.Count > 1).ToList();
                foreach (List<Operation> component in components)
                {
                    var componentHashSet = component.ToHashSet();
                    foreach (var operation1 in component)
                    {
                        var neighborOperations = GetNeighborComponentOperations(graph, operation1, componentHashSet);

                        foreach (var operation2 in neighborOperations)
                        {
                            // two operations are not on the same job and  operation 1 is after operation 2 (and edge is between them ofc)
                            if (operation1.JobId != operation2.JobId && operation1.Order > operation2.Order && operation1.MachineId == operation2.MachineId)
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