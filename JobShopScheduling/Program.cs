using System.Linq;
using OxyPlot;
using OxyPlot.Series;

namespace JobShopScheduling
{
    using JobShopStructures;

    internal class Program
    {
        private static void Main(string[] args)
        {
            string inputName = Global.Config.InputFileName;
            string inputPath = $"{Global.Config.InputFileDirectoryPath}/{inputName}";
            int iterationsCount = Global.Config.IterationsCount;
            
            JobShop jobShop = LoadJobShop(inputPath);
            //JobShop jobShop = GenerateJobShop();
            Global.Config.InversionMutationPerGeneProbability = 0.5f / jobShop.MachinesCount;
            
            var plottingUtils = new PlottingHelper();

            var gaAdaptive = new JobShopGeneticAlgorithm(jobShop, iterationsCount, adaptive: true)
            {
                PlotModel = plottingUtils.CreatePlotModel()
            };
            gaAdaptive.Run();
            var adaptiveSeries = plottingUtils.AverageY(gaAdaptive.PlotModel.Series.OfType<LineSeries>());
            adaptiveSeries.Title = "Adaptive";
            
            var gaNonAdaptive = new JobShopGeneticAlgorithm(jobShop, iterationsCount, adaptive: false)
            {
                PlotModel = plottingUtils.CreatePlotModel()
            };
            gaNonAdaptive.Run();
            var nonAdaptiveSeries = plottingUtils.AverageY(gaNonAdaptive.PlotModel.Series.OfType<LineSeries>());
            nonAdaptiveSeries.Title = "Basic";

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
