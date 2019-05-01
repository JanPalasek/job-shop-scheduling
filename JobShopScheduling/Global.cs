namespace JobShopScheduling
{
    /// <summary>
    /// Global static class for entire program.
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// Configuration of the program.
        /// </summary>
        public static Config Config { get; set; } = new Config(true);
    }
}