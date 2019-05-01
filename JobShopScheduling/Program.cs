namespace JobShopScheduling
{
    using JobShopStructures;

    internal class Program
    {
        private static void Main(string[] args)
        {
            JobShop jobShop = LoadJobShop("Examples/ft06.in");
            //JobShop jobShop = GenerateJobShop();

            var jobShopGeneticAlgorithm = new JobShopGeneticAlgorithm(jobShop);
            jobShopGeneticAlgorithm.Run(10, adaptive: false);
        }

        private static JobShop GenerateJobShop()
        {
            var generator = new JobShopGenerator();

            JobShop jobShop = generator.Generate(Config.OperationCounts, Config.MachinesCount);

            return jobShop;
        }

        private static JobShop LoadJobShop(string inputPath)
        {
            JobShop jobShop = new JobShopLoader().Load(inputPath);

            return jobShop;
        }
    }
}
