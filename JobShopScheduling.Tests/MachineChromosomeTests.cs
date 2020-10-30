namespace JobShopScheduling.Tests
{
    using System.Linq;
    using GeneticAlgorithm;
    using GeneticSharp.Domain.Chromosomes;
    using JobShopStructures;
    using NUnit.Framework;

    [TestFixture]
    public class MachineChromosomeTests
    {
        private MachineChromosome machineChromosome;

        [SetUp]
        public void SetUp()
        {
            var jobShop = new JobShopLoader().Load("TestExamples/test1.in", false);

            machineChromosome = new MachineChromosome(new int[]
            {
                4, 5, 0, 2
            }.Select(x => jobShop.Operations[x]).ToArray());
        }

        [Test]
        public void CloneTest()
        {
            var cloned = machineChromosome.Clone();

            Assert.That(cloned, Is.Not.SameAs(machineChromosome));
            Assert.That(cloned.GetGenes(), Is.Not.SameAs(machineChromosome.GetGenes()));

            Gene[] genes = machineChromosome.GetGenes();
            Gene[] clonedGenes = cloned.GetGenes();
            for (var i = 0; i < genes.Length; i++)
            {
                Assert.That(genes[i], Is.Not.SameAs(clonedGenes[i]));
            }
        }
    }
}