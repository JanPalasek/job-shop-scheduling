using System;

namespace JobShopScheduling
{
    using System.Linq;

    /// <summary>
    /// Configuration class for the run.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Directory of file from which the input will be loaded.
        /// </summary>
        public string InputFileDirectoryPath { get; set; }
        
        /// <summary>
        /// Input file name that is supposed to be inside the <see cref="InputFileDirectoryPath"/>,
        /// that will be loaded.
        /// </summary>
        public string InputFileName { get; set; }
        
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
        public float InversionMutationPerGeneProbability { get; set; }
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

                InputFileDirectoryPath = "Examples";
                InputFileName = "la19.in";
                IterationsCount = 10;
                ThreadsCount = Environment.ProcessorCount / 2;
            
                #region GA parameters
            
                GenerationsCount = 1500;
            
                MinPopulationSize = 100;
                MaxPopulationSize = 100;
            
                MutationProbability = 0.3f;
                CrossoverProbability = 0.75f;
                // mutate 2 times per solution
                InversionMutationPerGeneProbability = 0.05f;
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
    }
}