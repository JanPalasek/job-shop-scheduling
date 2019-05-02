using System.Linq;
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
            if (args.Length > 0)
            {
                Global.Config.InputFileName = args[0];
            }

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

        private static JobShop GenerateJobShop()
        {
            var generator = new JobShopGenerator();

            JobShop jobShop = generator.Generate(Global.Config.OperationCounts, Global.Config.MachinesCount);

            return jobShop;
        }

        private static JobShop LoadJobShop(string inputPath)
        {
            JobShop jobShop = new JobShopLoader().Load(inputPath);

            return jobShop;
        }
    }
}
