namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GeneticSharp.Domain.Chromosomes;
    using Graph;
    using JobShopStructures;
    using Utils;

    /// <summary>
    /// Machine chromosomes represents order of operations that need to be performed on this particular machine.
    /// </summary>
    public class MachineChromosome : ChromosomeBase
    {
        public MachineChromosome(Operation[] operations) : base(operations.Length)
        {
            base.ReplaceGenes(0, operations.Select(x => new Gene(x)).ToArray());
        }

        public MachineChromosome(Gene[] genes) : base(genes.Length)
        {
            base.ReplaceGenes(0, genes);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            throw new NotSupportedException();
        }

        public override IChromosome CreateNew()
        {
            // shuffle genes
            var shuffledGenes = GetGenes().AsShuffledEnumerable().ToArray();
            
            return new MachineChromosome(shuffledGenes);
        }

        public override IChromosome Clone()
        {
            return new MachineChromosome(GetGenes());
        }

        public string GetOperationStrings()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var operation in GetMachineOperations())
            {
                sb.Append($"{operation.Id}, ");
            }

            return sb.ToString();
        }

        public IEnumerable<Operation> GetMachineOperations()
        {
            return GetGenes().Select(x => x.Value).Cast<Operation>().Where(x => x != null);
        }
    }
}