﻿using System;
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

            string inputName = Global.Config.InputFileName;
            string inputPath = $"{Global.Config.InputFileDirectoryPath}/{inputName}";
            int iterationsCount = Global.Config.IterationsCount;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File($"{inputName}.log")
                .CreateLogger();

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
            Parser.Default.ParseArguments<Options>(cmdArgs)
                .WithParsed(o =>
                {
                    if (!string.IsNullOrEmpty(o.InputFileDirectoryPath))
                    {
                        Global.Config.InputFileDirectoryPath = o.InputFileDirectoryPath;
                    }
                    
                    if (!string.IsNullOrEmpty(o.InputFileName))
                    {
                        Global.Config.InputFileName = o.InputFileName;
                    }

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
