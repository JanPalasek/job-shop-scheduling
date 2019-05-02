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
            Global.Config.InversionMutationPerGeneProbability = 0.5f / jobShop.MachinesCount;
            
            var plottingUtils = new PlottingHelper();

            var gaAdaptive = new JobShopGeneticAlgorithm(jobShop, iterationsCount, logger: new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File($"{inputName}_ada.log")
                .CreateLogger(), adaptive: true)
            {
                PlotModel = plottingUtils.CreatePlotModel()
            };
            gaAdaptive.Run();
            var adaptiveSeries = plottingUtils.AverageY(gaAdaptive.PlotModel.Series.OfType<LineSeries>());
            adaptiveSeries.Title = "Adaptive";
            plottingUtils.ExportPlotModelToSvg($"{inputName}_ada", gaAdaptive.PlotModel);

            var gaNonAdaptive = new JobShopGeneticAlgorithm(jobShop, iterationsCount, logger: new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File($"{inputName}_basic.log")
                .CreateLogger(), adaptive: false)
            {
                PlotModel = plottingUtils.CreatePlotModel()
            };
            gaNonAdaptive.Run();
            var nonAdaptiveSeries = plottingUtils.AverageY(gaNonAdaptive.PlotModel.Series.OfType<LineSeries>());
            nonAdaptiveSeries.Title = "Basic";
            plottingUtils.ExportPlotModelToSvg($"{inputName}_basic", gaNonAdaptive.PlotModel);

            var plotModel = plottingUtils.CreatePlotModel();
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
