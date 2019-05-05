namespace JobShopScheduling.Tests
{
    using System.Linq;
    using GeneticAlgorithm;
    using GeneticSharp.Domain.Chromosomes;
    using GeneticSharp.Domain.Randomizations;
    using JobShopStructures;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ScheduleChromosomeTests
    {
        private ScheduleChromosome chromosome;

        [SetUp]
        public void SetUp()
        {
            var jobShop = new JobShopLoader().Load("TestExamples/test1.in");
            chromosome = new ScheduleChromosome(jobShop);

            var machineChromosome1 = new MachineChromosome(new int[]
            {
                4, 5, 0, 2
            }.Select(x => jobShop.Operations[x]).ToArray());
            chromosome.ReplaceGene(0, new Gene(machineChromosome1));

            var machineChromosome2 = new MachineChromosome(new int[]
            {
                1, 3
            }.Select(x => jobShop.Operations[x]).ToArray());
            chromosome.ReplaceGene(1, new Gene(machineChromosome2));
        }

        [Test]
        public void FixChromosomeTest()
        {
            var randomizationProviderMock = new Mock<IRandomization>();
            randomizationProviderMock.Setup(x => x.GetDouble()).Returns(Global.Config.BackEdgeSwitchOrientationProbability - 10e-6);
            // fix random sort order of AsShuffledEnumerable
            randomizationProviderMock.Setup(x => x.GetInt(0, int.MaxValue)).Returns(0);
            RandomizationProvider.Current = randomizationProviderMock.Object;

            // breaking the cycle switches orientation of edges (4, 0) and (5, 0)
            chromosome.FixChromosome();

            var topologicalOrder = chromosome.TopologicalOrder;
            var operations = chromosome.JobShop.Operations;

            Assert.That(topologicalOrder[0], Is.EqualTo(new Operation(int.MinValue, 0, 0, 0, 0)));
            Assert.That(topologicalOrder[1], Is.EqualTo(operations[0]));
            Assert.That(topologicalOrder[2], Is.EqualTo(operations[1]));
            Assert.That(topologicalOrder[3], Is.EqualTo(operations[3]));
            Assert.That(topologicalOrder[4], Is.EqualTo(operations[4]));
            Assert.That(topologicalOrder[5], Is.EqualTo(operations[5]));
            Assert.That(topologicalOrder[6], Is.EqualTo(operations[2]));
            Assert.That(topologicalOrder[7], Is.EqualTo(new Operation(int.MaxValue, 0, 0, 0, 0)));

            var machineChromosomes = chromosome.GetGenes().Select(x => x.Value).Cast<MachineChromosome>().ToList();

            var machineChromosome1 = machineChromosomes[0].GetGenes().Select(x => x.Value).Cast<Operation>().ToList();
            Assert.That(machineChromosome1[0], Is.EqualTo(operations[0]));
            Assert.That(machineChromosome1[1], Is.EqualTo(operations[4]));
            Assert.That(machineChromosome1[2], Is.EqualTo(operations[5]));
            Assert.That(machineChromosome1[3], Is.EqualTo(operations[2]));

            var machineChromosome2 = machineChromosomes[1].GetGenes().Select(x => x.Value).Cast<Operation>().ToList();
            Assert.That(machineChromosome2[0], Is.EqualTo(operations[1]));
            Assert.That(machineChromosome2[1], Is.EqualTo(operations[3]));
        }

        [Test]
        public void CloneTest()
        {
            var clonedChromosome = this.chromosome.Clone();
            Gene[] clonedGenes = clonedChromosome.GetGenes();
            Gene[] genes = chromosome.GetGenes();
            for (int i = 0; i < clonedGenes.Length; i++)
            {
                var machine = (MachineChromosome)genes[i].Value;
                var clonedMachine = (MachineChromosome)clonedGenes[i].Value;

                Assert.That(machine, Is.Not.SameAs(clonedMachine));
                Assert.That(machine.GetGenes(), Is.Not.SameAs(clonedMachine.GetGenes()));

                Gene[] machineGenes = machine.GetGenes();
                Gene[] clonedMachineGenes = clonedMachine.GetGenes();
                for (var j = 0; j < machineGenes.Length; j++)
                {
                    var machineGene = (Operation)machineGenes[j].Value;
                    var clonedMachineGene = (Operation)clonedMachineGenes[j].Value;

                    Assert.That(machineGene, Is.SameAs(clonedMachineGene));
                    Assert.That(machineGenes[j], Is.Not.SameAs(clonedMachineGenes[j]));
                }
            }
        }

        [Test]
        public void CreateNewTest()
        {
            var clonedChromosome = this.chromosome.CreateNew();
            Gene[] clonedGenes = clonedChromosome.GetGenes();
            Gene[] genes = chromosome.GetGenes();
            for (int i = 0; i < clonedGenes.Length; i++)
            {
                var machine = (MachineChromosome)genes[i].Value;
                var clonedMachine = (MachineChromosome)clonedGenes[i].Value;

                Assert.That(machine, Is.Not.SameAs(clonedMachine));
                Assert.That(machine.GetGenes(), Is.Not.SameAs(clonedMachine.GetGenes()));
            }
        }
    }
}