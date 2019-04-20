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
        public JobShop JobShop { get; private set; }

        private DiGraph<Operation> graph;

        public double? ScheduleLength { get; set; }

        public ScheduleChromosome(JobShop jobShop) : base(jobShop.MachinesCount)
        {
            this.JobShop = jobShop;

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
            return new ScheduleChromosome(JobShop);
        }

        public override IChromosome Clone()
        {
            var scheduleChromosome = (ScheduleChromosome)base.Clone();
            scheduleChromosome.JobShop = this.JobShop;
            scheduleChromosome.ScheduleLength = this.ScheduleLength;
            scheduleChromosome.graph = this.graph;
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

            var graphHandler = new GraphHandler();
            var graph = graphHandler.CreateGraph(this);
            var edgesThatChangedOrientation = graphHandler.BreakCycles(graph);

            // update chromosomes accordingly
            foreach ((Operation Operation1, Operation Operation2) edge in edgesThatChangedOrientation)
            {
                int machineId = edge.Operation1.MachineId;
                var machineChromosome = (MachineChromosome)GetGene(machineId).Value;

                machineChromosome.SwitchEdgeOrientation(edge);
            }


            this.graph = graph;

            return graph;
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

        public override string ToString()
        {
            return $"{ScheduleLength}";
        }
    }
}