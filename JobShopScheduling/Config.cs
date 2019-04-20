namespace JobShopScheduling
{
    using System.Linq;

    public static class Config
    {
        public static int MinPopulationSize { get; }
        public static int MaxPopulationSize { get; }
        public static int[] OperationCounts { get; }
        public static int MachinesCount { get; }
        public static int GenerationsCount { get; }

        public static float MutationProbability { get; }
        public static float CrossoverProbability { get; }
        public static float MutationPerBitProbability { get; }
        public static float TournamentSelectionProbability { get; }
        public static float ReinsertTournamentProbability { get; }
        public static float ElitismPercent { get; }

        public static float BackEdgeSwitchOrientationProbability { get; }
        public static float NormalEdgeSwitchOrientationProbability { get; }


        static Config()
        {
            MinPopulationSize = 100;
            MaxPopulationSize = 105;
            OperationCounts = new int[]{
                10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
                10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
            };
            MachinesCount = 10;
            GenerationsCount = 1000;

            MutationProbability = 0.3f;
            CrossoverProbability = 0.75f;
            // mutate 2 times per solution
            MutationPerBitProbability = 0.02f;
            ElitismPercent = 0.02f;

            TournamentSelectionProbability = 0.8f;
            ReinsertTournamentProbability = 0.7f;

            BackEdgeSwitchOrientationProbability = 0.95f;
            NormalEdgeSwitchOrientationProbability = 0.05f;
        }
    }
}