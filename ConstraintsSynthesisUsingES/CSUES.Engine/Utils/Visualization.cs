using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using ES.Core.Constraints;
using ES.Core.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace ES.Core.Utils
{
    public class Visualization
    {
        private readonly RangeColorAxis _colorAxis;
        private readonly Dictionary<OxyColor, double> _colorKey;
        private const string ColorAxisName = "ColorAxis";

        public Visualization()
        {
            Plots = new List<PlotView>();

            _colorKey = new Dictionary<OxyColor, double>();
            _colorAxis = new RangeColorAxis {Key = ColorAxisName};

            var fieldInfos = typeof(OxyColors).GetFields(BindingFlags.Static | BindingFlags.Public);
            var rangeStart = 0.0;

            foreach (var fieldInfo in fieldInfos)
            {
                var oxyColor = (OxyColor)fieldInfo.GetValue(null);

                if (_colorKey.ContainsKey(oxyColor)) continue;

                _colorAxis.AddRange(rangeStart, rangeStart + 0.1, oxyColor);
                _colorKey.Add(oxyColor, rangeStart);
                rangeStart++;
            }

            Application.EnableVisualStyles();
        }

        public List<PlotView> Plots { get; set; }
      
        public Thread Show()
        {

            var c = Plots.Count;

            var h = 500 + ((c - 1) / 3) * 400;
            int w;
            if (c > 2) w = 1300;
            else w = 100 + c * 400;

            var form = new Form()
            {
                Text = "Thesis",
                Height = h,
                Width = w,
                AutoScroll = true
            };

            var i = 0;

            foreach (var plot in Plots)
            {
                plot.Location = new System.Drawing.Point((i % 3) * 400, 20 + (i / 3) * 400);
                form.Controls.Add(plot);
                i++;
            }

            var plotThread = new Thread(() =>
            {
                Application.Run(form);
            });

            plotThread.SetApartmentState(ApartmentState.STA);
            plotThread.Start();
            
            return plotThread;
        }
        public Visualization AddNextPlot(string title = "Plot", int width = 400, int height = 400, int yAxisMin = -100, int yAxisMax = 100, int xAxisMin = -100, int xAxisMax = 100)
        {
            var plot = new PlotView { Size = new System.Drawing.Size(width, height) };

            var model = new PlotModel { Title = title };
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = xAxisMin, Maximum = xAxisMax });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = yAxisMin, Maximum = yAxisMax });
            model.Axes.Add(_colorAxis.DeepCopyByExpressionTree());
            plot.Model = model;

            Plots.Add(plot);

            return this;
        }

        public Visualization AddPoints(IEnumerable<Point> points, OxyColor color, MarkerType markerType = MarkerType.Circle, double pointSize = 3)
        {
            var plot = Plots.Last();

            var series = new ScatterSeries { MarkerType = markerType, ColorAxisKey = ColorAxisName };

            foreach (var point in points)
            {
                series.Points.Add(new ScatterPoint(point.Coordinates[0], point.Coordinates[1], pointSize, _colorKey[color]));
            }

            plot.Model.Series.Add(series);

            return this;
        }

        public Visualization AddConstraints(IList<Constraint> constraints, Func<int, OxyPalette> paletteInitializer = null, OxyColor color = default(OxyColor), double xMin = -100, double xMax = 100, double step = 0.5)
        {
            var plot = Plots.Last();

            OxyPalette palette = null;        

            if (paletteInitializer != null)
            {
                palette = paletteInitializer.Invoke(constraints.Count == 1 ? constraints.Count + 1 : constraints.Count);
            }
            else
            {
                color = color == default(OxyColor) ? OxyColors.Black : color;
            }

            for (var i = 0; i < constraints.Count; i++)
            {
                var denominator = constraints[i].TermsCoefficients[1];
                var aNominator = constraints[i].TermsCoefficients[0];
                var bNominator = constraints[i].LimitingValue;

                if (denominator == 0)
                {
                    denominator = 1;
                    aNominator *= 10000;
                    bNominator *= 10000;
                }

                var a = aNominator / denominator;
                var b = bNominator / denominator;

                var series = new FunctionSeries(x => b - a * x, xMin, xMax, step)
                {
                    Color = palette?.Colors[i] ?? color,
                };

                plot.Model.Series.Add(series);
            }

            return this;
        }

        public void ShowTwoPlots(IList<Point> positivePoints, IList<Point> negativePoints, MathModel mathModel)
        {
            this
                .AddNextPlot()
                .AddPoints(positivePoints, OxyColors.Green)
                .AddPoints(negativePoints, OxyColors.Red)
                .AddConstraints(mathModel.ReferenceModel, OxyPalettes.Rainbow, xMin: mathModel.Domains[0].LowerLimit, xMax: mathModel.Domains[0].UpperLimit)
                .AddNextPlot()
                .AddPoints(positivePoints, OxyColors.Green)
                .AddPoints(negativePoints, OxyColors.Red)
                .AddConstraints(mathModel.SynthesizedModel, OxyPalettes.Rainbow)
                .Show();
        }
    }
}
