namespace JobShopScheduling.GeneticAlgorithm
{
    using System;
    using System.Linq;
    using GeneticSharp.Domain.Chromosomes;

    public class MachineChromosome : ChromosomeBase
    {
        public MachineChromosome(int[] operationIds) : base(operationIds.Length)
        {
            base.ReplaceGenes(0, operationIds.Select(x => new Gene(x)).ToArray());
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
    }
}