namespace JobShopScheduling
{
    public static class Config
    {
        public static float MutationProbability { get; }
        public static float CrossoverProbability { get; }
        public static float MutationPerBitProbability { get; }
        public static float TournamentProbability { get; }
        public static float ElitismPercent { get; }

        public static float BackEdgeSwitchOrientationProbability { get; }
        public static float NormalEdgeSwitchOrientationProbability { get; }


        static Config()
        {
            MutationProbability = 0.4f;
            CrossoverProbability = 0.75f;
            MutationPerBitProbability = 0.05f;
            ElitismPercent = 0.04f;
            TournamentProbability = 0.9f;

            BackEdgeSwitchOrientationProbability = 0.75f;
            NormalEdgeSwitchOrientationProbability = 0.2f;
        }
    }
}