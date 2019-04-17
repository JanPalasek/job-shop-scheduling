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

            // topologically sort the graph => returns topologically sorted operations
            var topologicallySortedOperations = new DepthFirstTopSort<Operation>().GetTopSort(graph);
            
            //
            double[] highestOperationCosts = new double[topologicallySortedOperations.Count];

            // find longest distance to i-th operation for every i
            for (int i = 1; i < topologicallySortedOperations.Count; i++)
            {
                // check all edges to the i-th vertex
                var operation = topologicallySortedOperations[i];
                for (int j = 0; j < i; j++)
                {
                    var otherOperation = topologicallySortedOperations[j];

                    if (graph.HasEdge(otherOperation, operation))
                    {
                        highestOperationCosts[i] = Math.Max(highestOperationCosts[i],
                            highestOperationCosts[j] + otherOperation.Cost);
                    }
                }
            }

            AssertLastIsMaximum(highestOperationCosts);

            double scheduleLength = highestOperationCosts[highestOperationCosts.Length - 1];
            chromosome.ScheduleLength = scheduleLength;
            chromosome.Fitness = 1 / (scheduleLength + 1);

            return chromosome.Fitness.Value;
        }

        private void AssertLastIsMaximum(double[] values)
        {
            double lastValue = values[values.Length - 1];

            if (values.Any(x => x > lastValue))
            {
                throw new ArgumentException();
            }
        }
    }
}