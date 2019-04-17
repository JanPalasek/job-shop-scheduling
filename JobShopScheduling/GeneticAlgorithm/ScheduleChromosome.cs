namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using Advanced.Algorithms.DataStructures.Foundation;
    using Advanced.Algorithms.DataStructures.Graph.AdjacencyList;
    using Advanced.Algorithms.Graph;
    using GeneticSharp.Domain.Chromosomes;

    public class ScheduleChromosome : ChromosomeBase
    {
        /// <summary>
        /// List of jobs that have to be done.
        /// </summary>
        private JobShop jobShop;

        private DiGraph<Operation> graph;

        public double? ScheduleLength { get; set; }

        public ScheduleChromosome(JobShop jobShop) : base(jobShop.MachinesCount)
        {
            this.jobShop = jobShop;

            // initialize genes
            for (int i = 0; i < jobShop.MachinesCount; i++)
            {
                var machineChromosome = new MachineChromosome(jobShop.MachineOperations[i]
                    .AsShuffledEnumerable()
                    .ToArray());
                ReplaceGene(i, new Gene(machineChromosome));
            }
        }

        public override Gene GenerateGene(int geneIndex)
        {
            var machineChromosome = (MachineChromosome)GetGenes()[geneIndex].Value;
            var shuffled = machineChromosome.GetGenes().AsShuffledEnumerable().ToArray();
            var newMachineChromosome = new MachineChromosome(shuffled);
            return new Gene(newMachineChromosome);
        }

        public override IChromosome CreateNew()
        {
            return new ScheduleChromosome(jobShop);
        }

        public override IChromosome Clone()
        {
            var scheduleChromosome = (ScheduleChromosome)base.Clone();
            scheduleChromosome.jobShop = this.jobShop;
            scheduleChromosome.ScheduleLength = this.ScheduleLength;
            return scheduleChromosome;
        }

        public DiGraph<Operation> ToDirectedGraph()
        {
            // fitness is not null => cycles are fixed already => graph is correctly cached
            // whenever some operation changes the element, fitness is reset
            if (Fitness != null)
            {
                return this.graph;
            }

            // dictionary mapping first operation to its successor
            var machineOperationsDictionary = new System.Collections.Generic.Dictionary<Operation, Operation>();
            foreach (var machineChromosome in GetGenes().Select(x => x.Value).Cast<MachineChromosome>().Where(x => x.RealLength >= 2))
            {
                var machineOperations = machineChromosome.GetGenes().Select(x => x.Value).Cast<Operation>().ToArray();
                for (int i = 0; i < machineOperations.Length - 1; i++)
                {
                    machineOperationsDictionary.Add(machineOperations[i], machineOperations[i + 1]);
                }
            }

            var graph = new DiGraph<Operation>();

            var source = graph.AddVertex(new Operation(int.MinValue, int.MinValue, int.MinValue, int.MinValue, 0));
            var target = graph.AddVertex(new Operation(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, 0));
            foreach (var job in jobShop.Jobs)
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
            foreach (var operation in jobShop.Operations)
            {
                if (machineOperationsDictionary.TryGetValue(operation, out var nextMachineOperation))
                {
                    if (nextMachineOperation.JobId != operation.JobId)
                    {
                        graph.AddEdge(operation, nextMachineOperation);
                    }
                }
            }

            // break cycles
            var edgesThatChangedOrientation = new CycleBreaker(Config.BackEdgeSwitchOrientationProbability,
                Config.NormalEdgeSwitchOrientationProbability).BreakCycles(graph);

            // update chromosomes accordingly
            foreach ((Operation Operation1, Operation Operation2) edge in edgesThatChangedOrientation)
            {
                int machineId = edge.Operation1.MachineId;
                var machineChromosome = (MachineChromosome) GetGene(machineId).Value;

                machineChromosome.UpdateEdgeOrientation(edge);
            }

            this.graph = graph;

            //AssertNoCycle(graph);

            return graph;
        }

        private void AssertNoCycle(DiGraph<Operation> graph)
        {
            if (new CycleDetector<Operation>().HasCycle(graph))
            {
                throw new ArgumentException("Cycle detected.");
            }
        } 

        public string GetOperationStrings()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var machineChromosome in GetGenes().Select(x => x.Value).Cast<MachineChromosome>())
            {
                sb.AppendLine(machineChromosome.GetOperationStrings());
            }

            return sb.ToString();
        }
    }
}