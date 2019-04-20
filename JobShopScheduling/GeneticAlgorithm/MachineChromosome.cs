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

    public class MachineChromosome : ChromosomeBase
    {
        public int RealLength { get; }

        public MachineChromosome(Operation[] operations) : base(operations.Length < 2 ? 2 : operations.Length)
        {
            RealLength = operations.Length;
            base.ReplaceGenes(0, operations.Select(x => new Gene(x)).ToArray());
        }

        public MachineChromosome(Gene[] genes) : base(genes.Length < 2 ? 2 : genes.Length)
        {
            RealLength = genes.Length;
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

        private IEnumerable<Operation> GetMachineOperations()
        {
            return GetGenes().Select(x => x.Value).Cast<Operation>().Where(x => x != null);
        }
    }
}