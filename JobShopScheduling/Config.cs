namespace JobShopScheduling
{
    public static class Config
    {
        public static float MutationProbability { get; }
        public static float CrossoverProbability { get; }
        public static float MutationPerBitProbability { get; }
        public static float ElitismPercent { get; }

        public static float BackEdgeSwitchOrientationProbability { get; }
        public static float NormalEdgeSwitchOrientationProbability { get; }


        static Config()
        {
            MutationProbability = 0.6f;
            CrossoverProbability = 0.75f;
            MutationPerBitProbability = 0.05f;
            ElitismPercent = 0.01f;

            BackEdgeSwitchOrientationProbability = 0.7f;
            NormalEdgeSwitchOrientationProbability = 0.3f;
        }
    }
}