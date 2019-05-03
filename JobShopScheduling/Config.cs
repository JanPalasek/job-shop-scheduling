using System;

namespace JobShopScheduling
{
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Configuration class for the run.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Input file path.
        /// </summary>
        public string InputFilePath { get; set; }
        
        public int IterationsCount { get; set; }
        
        public int ThreadsCount { get; set; }
        public int MinPopulationSize { get; set; }
        public int MaxPopulationSize { get; set; }
        public int[] OperationCounts { get; set; }
        public int MachinesCount { get; set; }
        public int GenerationsCount { get; set; }

        public float MutationProbability { get; set; }
        
        public float MinimumMutationProbability { get; set; }
        public float MaximumMutationProbability { get; set; }
        public float CrossoverProbability { get; set; }
        public float MutationPerGeneProbability { get; set; }
        public float TournamentSelectionProbability { get; set; }
        public float ElitismPercent { get; set; }

        public float BackEdgeSwitchOrientationProbability { get; set; }
        public float NormalEdgeSwitchOrientationProbability { get; set; }


        public Config(bool initializeDefaultValues)
        {
            if (initializeDefaultValues)
            {
                #region Input for generator
            
                OperationCounts = new int[]{
                    10, 10, 10, 10, 10, 10, 10, 10, 10
                };
                MachinesCount = 10;
            
                #endregion

                IterationsCount = 10;
                ThreadsCount = Environment.ProcessorCount / 2;
            
                #region GA parameters
            
                GenerationsCount = 1500;
            
                MinPopulationSize = 100;
                MaxPopulationSize = 100;
            
                MutationProbability = 0.3f;
                CrossoverProbability = 0.75f;
                // mutate 2 times per solution
                MutationPerGeneProbability = 0.05f;
                ElitismPercent = 0.02f;

                TournamentSelectionProbability = 0.8f;

                BackEdgeSwitchOrientationProbability = 0.95f;
                NormalEdgeSwitchOrientationProbability = 0.05f;
            
                #endregion
            
                #region Adaptive

                MinimumMutationProbability = 0.2f;
                MaximumMutationProbability = 0.5f;

                #endregion
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Input file path: {InputFilePath}");
            sb.AppendLine($"Iterations count: {IterationsCount}");
            sb.AppendLine($"Generations count: {GenerationsCount}");
            sb.AppendLine($"Threads count: {ThreadsCount}");
            sb.AppendLine($"Min population size: {MinPopulationSize}");
            sb.AppendLine($"Max population size: {MaxPopulationSize}");

            sb.AppendLine($"Crossover probability: {CrossoverProbability:F}");
            sb.AppendLine($"Mutation probability: {MutationProbability:F}");
            sb.AppendLine($"Mutation per gene probability: {MutationPerGeneProbability:F}");
            sb.AppendLine($"Elitism percent: {ElitismPercent}");

            return sb.ToString();
        }
    }
}