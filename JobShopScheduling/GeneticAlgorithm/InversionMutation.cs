using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;

namespace JobShopScheduling.GeneticAlgorithm
{
    /// <summary>
    /// Inversion mutation that can be performed on <see cref="MachineChromosome"/>.
    /// </summary>
    public class InversionMutation : IMutation
    {
        public bool IsOrdered { get; } = true;

        private void Mutate(MachineChromosome chromosome, float probability)
        {
            if (RandomizationProvider.Current.GetDouble() <= probability)
            {
                if (chromosome.RealLength == 1)
                {
                    return;
                }

                // generate two random different indices
                int firstIndex, secondIndex;
                do
                {
                    firstIndex = RandomizationProvider.Current.GetInt(0, chromosome.RealLength);
                    secondIndex = RandomizationProvider.Current.GetInt(0, chromosome.RealLength);
                } while (firstIndex == secondIndex);
                
                // make first index to be the lower one
                if (firstIndex > secondIndex)
                {
                    int temp = firstIndex;
                    firstIndex = secondIndex;
                    secondIndex = temp;
                }

                var reversedGenes = chromosome.GetGenes().Skip(firstIndex).Take(secondIndex - firstIndex + 1)
                    .Reverse().ToList();

                for (int i = firstIndex, j = 0; i <= secondIndex; i++, j++)
                {
                    chromosome.ReplaceGene(i, new Gene(reversedGenes[j].Value));
                }
            }
        }
        
        public void Mutate(IChromosome chromosome, float probability)
        {
            Mutate((MachineChromosome)chromosome, probability);
        }
    }
}