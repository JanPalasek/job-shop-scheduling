namespace JobShopScheduling
{
    using System.Linq;

    /// <summary>
    /// Configuration class for the run.
    /// </summary>
    public static class Config
    {
        public static int MinPopulationSize { get; }
        public static int MaxPopulationSize { get; }
        public static int[] OperationCounts { get; }
        public static int MachinesCount { get; }
        public static int GenerationsCount { get; }

        public static float MutationProbability { get; }
        
        public static float MinimumMutationProbability { get; }
        public static float MaximumMutationProbability { get; }
        public static float CrossoverProbability { get; }
        public static float InversionMutationPerGeneProbability { get; }
        public static float TournamentSelectionProbability { get; }
        public static float ElitismPercent { get; }

        public static float BackEdgeSwitchOrientationProbability { get; }
        public static float NormalEdgeSwitchOrientationProbability { get; }


        static Config()
        {
            #region Input for generator
            
            OperationCounts = new int[]{
                10, 10, 10, 10, 10, 10, 10, 10, 10
            };
            MachinesCount = 10;
            
            #endregion
            
            #region Population parameters
            
            MinPopulationSize = 100;
            MaxPopulationSize = 100;
            GenerationsCount = 1000;
            
            #endregion

            MutationProbability = 0.3f;
            CrossoverProbability = 0.75f;
            // mutate 2 times per solution
            InversionMutationPerGeneProbability = 0.5f / MachinesCount;
            ElitismPercent = 0.02f;

            TournamentSelectionProbability = 0.8f;

            BackEdgeSwitchOrientationProbability = 0.95f;
            NormalEdgeSwitchOrientationProbability = 0.05f;
            
            #region Adaptive

            MinimumMutationProbability = 0.2f;
            MaximumMutationProbability = 0.5f;

            #endregion
        }
    }
}