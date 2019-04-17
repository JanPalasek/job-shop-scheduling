namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using System.Linq;
    using Advanced.Algorithms.Graph;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Fitnesses;

    public class ScheduleFitness : IFitness
    {
        public double Evaluate(IChromosome chromosome)
        {
            return Evaluate((ScheduleChromosome) chromosome);
        }

        private double Evaluate(ScheduleChromosome chromosome)
        {
            // create graph
            var graph = chromosome.ToDirectedGraph();

            // topologically sort the graph
            var topologicallySortedOperations = new DepthFirstTopSort<Operation>().GetTopSort(graph);
            
            double[] highestOperationCosts = new double[topologicallySortedOperations.Count];

            // find longest path
            for (int i = 0; i < topologicallySortedOperations.Count - 1; i++)
            {
                // check all edges from the i-th vertex
                var operation = topologicallySortedOperations[i];
                for (int j = i + 1; j < topologicallySortedOperations.Count; j++)
                {
                    var otherOperation = topologicallySortedOperations[j];

                    if (graph.HasEdge(operation, otherOperation))
                    {
                        highestOperationCosts[j] = Math.Max(highestOperationCosts[j],
                            highestOperationCosts[i] + operation.Cost);
                    }
                }
            }

            double scheduleLength = highestOperationCosts[highestOperationCosts.Length - 1];
            chromosome.ScheduleLength = scheduleLength;
            chromosome.Fitness = 1 / (scheduleLength + 1);

            return chromosome.Fitness.Value;
        }
    }
}