using System.Collections.Generic;
using System.IO;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace JobShopScheduling
{
    /// <summary>
    /// Helper class for plotting.
    /// </summary>
    public class PlottingHelper
    {
        /// <summary>
        /// Takes list of line series in the input, calculates average for every Y coordinate and returns this merged
        /// result as line series in the output.
        /// </summary>
        /// <param name="lineSeries">List of line series in the input. Must have at least 1 element.</param>
        /// <returns></returns>
        public LineSeries AverageY(IEnumerable<LineSeries> lineSeries)
        {
            var newLineSeries = new LineSeries();
            
            var firstLine = lineSeries.First();
            int pointsCount = firstLine.Points.Count;
            for (int i = 0; i < pointsCount; i++)
            {
                double averageY = lineSeries.Select(x => x.Points[i]).Average(x => x.Y);
                
                newLineSeries.Points.Add(new DataPoint(firstLine.Points[i].X, averageY));
            }

            return newLineSeries;
        }

        /// <summary>
        /// Merges list of plot models into one with configuration taken from the first one in the list.
        /// </summary>
        /// <param name="plotModels"></param>
        /// <returns></returns>
        public PlotModel MergePlotModels(params PlotModel[] plotModels)
        {
            var firstPlotModel = plotModels[0];
            var newPlotModel = new PlotModel();
            
            // copy axes from the first plot model
            foreach (var axis in firstPlotModel.Axes)
            {
                newPlotModel.Axes.Add(axis);
            }
            
            // copy all series from all plot models
            foreach (var plotModel in plotModels)
            {
                foreach (var series in plotModel.Series)
                {
                    newPlotModel.Series.Add(series);
                }
            }

            return newPlotModel;
        }

        /// <summary>
        /// Exports specified model into SVG format.
        /// </summary>
        /// <param name="exportFilePath"></param>
        /// <param name="plotModel"></param>
        public void ExportPlotModelToSvg(string exportFilePath, PlotModel plotModel)
        {
            using (var stream = File.Create($"{exportFilePath}.svg"))
            {
                var exporter = new SvgExporter { Width = 600, Height = 400 };
                exporter.Export(plotModel, stream);
            }
        }

        /// <summary>
        /// Creates plot model with some basic configuration for plotting GA.
        /// </summary>
        /// <returns></returns>
        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel();
            plotModel.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, Title = "Schedule length"});
            plotModel.Axes.Add(new LogarithmicAxis() { Position = AxisPosition.Bottom, Title = "Generation" });
            return plotModel;
        }
    }
}