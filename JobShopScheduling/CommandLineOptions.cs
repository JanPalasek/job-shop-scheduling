using CommandLine;

namespace JobShopScheduling
{
    /// <summary>
    /// Options from command line.
    /// </summary>
    public class CommandLineOptions
    {
        [Option('f', "file", Required = true, HelpText = "Name of input file.")]
        public string InputFileName { get; set; }
            
        [Option('d', "dir", Required = false, HelpText = "Directory of the input file.")]
        public string InputFileDirectoryPath { get; set; }
            
        [Option('t', "threads", Required = false, HelpText = "Number of threads that will be used.")]
        public int? ThreadsCount { get; set; }
            
        [Option('g', "gen", Required = false, HelpText = "Number of generations that will be run.")]
        public int? GenerationsCount { get; set; }
            
        [Option('i', "it", Required = false, HelpText = "Number of iterations that will be run.")]
        public int? IterationsCount { get; set; }
    }
}