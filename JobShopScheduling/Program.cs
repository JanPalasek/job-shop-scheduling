using System;
using System.IO;
using System.Linq;
using CommandLine;
using OxyPlot;
using OxyPlot.Series;

namespace JobShopScheduling
{
    using JobShopStructures;
    using Serilog;

    internal class Program
    {
        private static void Main(string[] args)
        {
            ParseCmdArguments(args);

            string inputPath = Global.Config.InputFilePath;
            string inputName = Path.GetFileName(inputPath);
            int iterationsCount = Global.Config.IterationsCount;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File($"{inputName}.log")
                .CreateLogger();

            Log.Information("CONFIG PARAMETERS");
            Log.Information(Global.Config.ToString());

            JobShop jobShop = LoadJobShop(inputPath);
            //JobShop jobShop = GenerateJobShop();
            
            var plottingUtils = new PlottingHelper();

            var gaAdaptive = new JobShopGeneticAlgorithm(jobShop, iterationsCount, logger: Log.Logger, adaptive: true)
            {
                PlotModel = plottingUtils.CreatePlotModel()
            };
            gaAdaptive.Run();
            var adaptiveSeries = plottingUtils.AverageY(gaAdaptive.PlotModel.Series.OfType<LineSeries>());
            adaptiveSeries.Title = "Adaptive";

            var gaNonAdaptive = new JobShopGeneticAlgorithm(jobShop, iterationsCount, logger: Log.Logger, adaptive: false)
            {
                PlotModel = plottingUtils.CreatePlotModel()
            };
            gaNonAdaptive.Run();
            var nonAdaptiveSeries = plottingUtils.AverageY(gaNonAdaptive.PlotModel.Series.OfType<LineSeries>());
            nonAdaptiveSeries.Title = "Basic";

            var plotModel = plottingUtils.CreatePlotModel();
            plotModel.Title = inputName;
            plotModel.Series.Add(adaptiveSeries);
            plotModel.Series.Add(nonAdaptiveSeries);
            
            plottingUtils.ExportPlotModelToSvg(inputName, plotModel);
        }

        /// <summary>
        /// Parses command-line arguments, storing info from them into <see cref="Global.Config"/>.
        /// </summary>
        /// <param name="cmdArgs"></param>
        private static void ParseCmdArguments(string[] cmdArgs)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(cmdArgs)
                .WithParsed(o =>
                {
                    Global.Config.InputFilePath = o.InputFilePath;

                    if (o.ThreadsCount != null)
                    {
                        Global.Config.ThreadsCount = o.ThreadsCount.Value;
                    }
                    
                    if (o.IterationsCount != null)
                    {
                        Global.Config.IterationsCount = o.IterationsCount.Value;
                    }
                    
                    if (o.GenerationsCount != null)
                    {
                        Global.Config.GenerationsCount = o.GenerationsCount.Value;
                    }
                })
                .WithNotParsed(o =>
                {
                    if (o.Any())
                    {
                        throw new ArgumentException("Error in command-line arguments.");
                    }
                });
        }
        
        private static JobShop GenerateJobShop()
        {
            var generator = new JobShopGenerator();

            JobShop jobShop = generator.Generate(Global.Config.OperationCounts, Global.Config.MachinesCount);

            return jobShop;
        }

        /// <summary>
        /// Loads <see cref="JobShop"/> from specified input path.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <returns></returns>
        private static JobShop LoadJobShop(string inputPath)
        {
            JobShop jobShop = new JobShopLoader().Load(inputPath);

            return jobShop;
        }
    }
}
