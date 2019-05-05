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
    using Graph;
    using JobShopStructures;
    using Utils;

    /// <summary>
    /// Genetic algorithm individual representing a schedule.
    /// </summary>
    public class ScheduleChromosome : ChromosomeBase
    {
        /// <summary>
        /// List of jobs that have to be done.
        /// </summary>
        public JobShop JobShop { get; private set; }

        internal DiGraph<Operation> Graph { get; private set; }
        internal IReadOnlyList<Operation> TopologicalOrder { get; private set; }

        public double? ScheduleLength { get; set; }

        public ScheduleChromosome(JobShop jobShop) : this(jobShop, false)
        {
        }

        private ScheduleChromosome(JobShop jobShop, bool generateNew) : base(jobShop.MachinesCount)
        {
            this.JobShop = jobShop;

            if (generateNew)
            {
                // initialize genes
                for (int i = 0; i < jobShop.MachinesCount; i++)
                {
                    var machineChromosome = new MachineChromosome(jobShop.MachineOperations[i]
                        .AsShuffledEnumerable()
                        .ToArray());
                    ReplaceGene(i, new Gene(machineChromosome));
                }
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
            var scheduleChromosome = new ScheduleChromosome(this.JobShop, false)
            {
                ScheduleLength = this.ScheduleLength,
                Graph = this.Graph,
                Fitness =  this.Fitness,
            };
            var clonedGenes = GetGenes().Select(x => x.Value)
                .Cast<MachineChromosome>().Select(x => x.Clone())
                .Select(x => new Gene(x)).ToArray();
            scheduleChromosome.ReplaceGenes(0, clonedGenes);

            return scheduleChromosome;
        }

        /// <summary>
        /// This method fixes the chromosome - there could be cycles before => this method removes the cycles
        /// using graph.
        /// </summary>
        public void FixChromosome()
        {
            if (Fitness != null)
            {
                return;
            }

            var graphHandler = new GraphHandler();
            var graph = graphHandler.CreateGraph(this);
            graphHandler.BreakCycles(graph);

            // update the chromosome => get topologically sorted graph and rewrite values in machine indices
            var topologicalOrder = graphHandler.GetTopologicalOrder(graph);
            var machineIndices = new int[JobShop.MachinesCount];
            var machineChromosomes = GetGenes().Select(x => x.Value).Cast<MachineChromosome>().ToList();
            foreach (var operation in topologicalOrder.Where(x => x.Id > int.MinValue && x.Id < int.MaxValue))
            {
                int machineId = operation.MachineId;
                ref int machineOperationIndex = ref machineIndices[machineId];

                machineChromosomes[machineId].ReplaceGene(machineOperationIndex, new Gene(operation));
                machineOperationIndex++;
            }

            this.Graph = graph;
            this.TopologicalOrder = topologicalOrder;
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