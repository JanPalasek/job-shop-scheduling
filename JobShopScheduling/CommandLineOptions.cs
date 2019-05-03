using CommandLine;

namespace JobShopScheduling
{
    /// <summary>
    /// Options from command line.
    /// </summary>
    public class CommandLineOptions
    {
        [Option('f', "file", Required = true, HelpText = "Path to the input file.")]
        public string InputFilePath { get; set; }
            
        [Option('t', "threads", Required = false, HelpText = "Number of threads that will be used.")]
        public int? ThreadsCount { get; set; }
            
        [Option('g', "gen", Required = false, HelpText = "Number of generations that will be run.")]
        public int? GenerationsCount { get; set; }
            
        [Option('i', "it", Required = false, HelpText = "Number of iterations that will be run.")]
        public int? IterationsCount { get; set; }
    }
}