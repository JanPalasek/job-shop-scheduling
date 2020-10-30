namespace JobShopScheduling.Tests
{
    using Graph;
    using JobShopStructures;
    using NUnit.Framework;

    [TestFixture]
    public class JobShopLoaderTests
    {
        [Test]
        public void LoadTest()
        {
            var loader = new JobShopLoader();
            var jobShop = loader.Load("TestExamples/test1.in", false);
            var job1 = jobShop.Jobs[0];
            var job2 = jobShop.Jobs[1];

            var operation0 = job1.Operations[0];
            Assert.That(operation0.Id, Is.EqualTo(0));
            Assert.That(operation0.Cost, Is.EqualTo(2));
            Assert.That(operation0.JobId, Is.EqualTo(0));
            Assert.That(operation0.MachineId, Is.EqualTo(0));

            var operation1 = job1.Operations[1];
            Assert.That(operation1.Id, Is.EqualTo(1));
            Assert.That(operation1.Cost, Is.EqualTo(5));
            Assert.That(operation1.JobId, Is.EqualTo(0));
            Assert.That(operation1.MachineId, Is.EqualTo(1));

            var operation2 = job1.Operations[2];
            Assert.That(operation2.Id, Is.EqualTo(2));
            Assert.That(operation2.Cost, Is.EqualTo(3));
            Assert.That(operation2.JobId, Is.EqualTo(0));
            Assert.That(operation2.MachineId, Is.EqualTo(0));

            var operation5 = job2.Operations[2];
            Assert.That(operation5.Id, Is.EqualTo(5));
            Assert.That(operation5.Cost, Is.EqualTo(4));
            Assert.That(operation5.JobId, Is.EqualTo(1));
            Assert.That(operation5.MachineId, Is.EqualTo(0));
        }
    }
}