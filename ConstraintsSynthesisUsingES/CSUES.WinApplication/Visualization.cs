using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CSUES.Engine.Enums;
using CSUES.Engine.Models;
using CSUES.Engine.Models.Constraints;
using ES.Core.Utils;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace CSUES.WinApplication
{
    public class Visualization
    {
        private readonly RangeColorAxis _colorAxis;
        private readonly Dictionary<OxyColor, double> _colorKey;
        private const string ColorAxisName = "ColorAxis";

        private readonly int _plotWidth;
        private readonly int _plotHeight;
        private readonly double _xAxisMin;
        private readonly double _xAxisMax;
        private readonly double _yAxisMin;
        private readonly double _yAxisMax;       

        public Visualization(BenchmarkType benchmarkType)
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

            switch (benchmarkType)
            {
                case BenchmarkType.Balln:
                    _plotWidth = 400;
                    _plotHeight = 400;
                    _xAxisMin = -5;
                    _xAxisMax = 7;
                    _yAxisMin = -4;
                    _yAxisMax = 8;
                    break;
                case BenchmarkType.Cuben:
                    _plotWidth = 400;
                    _plotHeight = 400;
                    _xAxisMin = -2;
                    _xAxisMax = 6.6;
                    _yAxisMin = -4.5;
                    _yAxisMax = 13.5;
                    break;
                case BenchmarkType.Simplexn:
                    _plotWidth = 400;
                    _plotHeight = 400;
                    _xAxisMin = -1.5;
                    _xAxisMax = 5;
                    _yAxisMin = -1.5;
                    _yAxisMax = 5;
                    break;
                case BenchmarkType.Other:
                    _plotWidth = 400;
                    _plotHeight = 400;
                    _xAxisMin = -100;
                    _xAxisMax = 100;
                    _yAxisMin = -100;
                    _yAxisMax = 100;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(benchmarkType), benchmarkType, null);
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

            var form = new Form
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
        public Visualization AddNextPlot(string title = "Plot", int width = 400, int height = 400, double yAxisMin = -100, double yAxisMax = 100, double xAxisMin = -100, double xAxisMax = 100)
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

        public Visualization AddNextPlot(string title = "Plot")
        {
            return AddNextPlot(title, _plotWidth, _plotHeight, _yAxisMin, _yAxisMax, _xAxisMin, _xAxisMax);
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
                Series series;

                if (constraints[i] is QuadraticConstraint)
                {
                    var a = constraints[i].Terms[2].Coefficient * -0.5;
                    var b = constraints[i].Terms[3].Coefficient * -0.5;
                    var r = Math.Sqrt(constraints[i].LimitingValue + (a * a) + (b * b));

                    series = new FunctionSeries(t => a + r * Math.Cos(t), t => b + r * Math.Sin(t), 0, 2 * Math.PI, 1000)
                    {
                        Color = palette?.Colors[i] ?? color
                    };
                }
                else
                {
                    var denominator = constraints[i].Terms[1].Coefficient;
                    var aNominator = constraints[i].Terms[0].Coefficient;
                    var bNominator = constraints[i].LimitingValue;

                    if (denominator == 0)
                    {
                        denominator = 1;
                        aNominator *= 10000;
                        bNominator *= 10000;
                    }

                    var a = aNominator / denominator;
                    var b = bNominator / denominator;

                    series = new FunctionSeries(x => b - a * x, xMin, xMax, step)
                    {
                        Color = palette?.Colors[i] ?? color,
                    };
                }

                plot.Model.Series.Add(series);
            }

            return this;
        }


        public Visualization AddEvolutionSteps(IList<IList<Constraint>> evolutionSteps, int numberOfSteps)
        {
            var stepIncrement = evolutionSteps.Count / numberOfSteps;
            var j = 0;

            for (var i = 0; i < numberOfSteps * stepIncrement; i += stepIncrement)
            {
                if (i > evolutionSteps.Count) break;

                var color = OxyColor.FromRgb(50, 50, (byte)(byte.MaxValue / numberOfSteps * j++));
                AddConstraints(evolutionSteps[i], null, color);
            }

            return this;
        }

        public Visualization PreparePlots(IList<Point> positivePoints, IList<Point> negativePoints, MathModel mathModel)
        {
            AddNextPlot("Reference model - Training points", _plotWidth, _plotHeight, _yAxisMin, _yAxisMax, _xAxisMin, _xAxisMax);
            AddPoints(positivePoints, OxyColors.Green);
            AddPoints(negativePoints, OxyColors.DarkRed);
            AddConstraints(mathModel.ReferenceModel, OxyPalettes.Rainbow, xMin: mathModel.Domains[0].LowerLimit,
                xMax: mathModel.Domains[0].UpperLimit);
            AddNextPlot("Synthesized model - Training points", _plotWidth, _plotHeight, _yAxisMin, _yAxisMax, _xAxisMin, _xAxisMax);
            AddPoints(positivePoints, OxyColors.Green);
            AddPoints(negativePoints, OxyColors.DarkRed);
            AddConstraints(mathModel.SynthesizedModel, OxyPalettes.Rainbow, xMin: mathModel.Domains[0].LowerLimit,
                xMax: mathModel.Domains[0].UpperLimit);

            return this;
        }

        public Visualization PreparePlots(IList<Point> testPoints, MathModel mathModel)
        {
            var positiveTestPoints = testPoints.Where(tp => tp.ClassificationType == ClassificationType.Positive).ToList();
            var negativeTestPoints = testPoints.Where(tp => tp.ClassificationType == ClassificationType.Negative).ToList();

            AddNextPlot("Reference model - Test points", _plotWidth, _plotHeight, _yAxisMin, _yAxisMax, _xAxisMin, _xAxisMax);
            AddPoints(positiveTestPoints, OxyColors.Green);
            AddPoints(negativeTestPoints, OxyColors.DarkRed);
            AddConstraints(mathModel.ReferenceModel, OxyPalettes.Rainbow, xMin: mathModel.Domains[0].LowerLimit,
                xMax: mathModel.Domains[0].UpperLimit);
            AddNextPlot("Synthesized model - Test points", _plotWidth, _plotHeight, _yAxisMin, _yAxisMax, _xAxisMin, _xAxisMax);
            AddPoints(positiveTestPoints, OxyColors.Green);
            AddPoints(negativeTestPoints, OxyColors.DarkRed);
            AddConstraints(mathModel.SynthesizedModel, OxyPalettes.Rainbow, xMin: mathModel.Domains[0].LowerLimit,
                xMax: mathModel.Domains[0].UpperLimit);

            return this;
        }

        public Visualization PreparePlots(IList<Point> positivePoints, IList<Point> negativePoints, MathModel mathModel, IList<IList<Constraint>> evolutionSteps, int numberOfSteps)
        {
            PreparePlots(positivePoints, negativePoints, mathModel);
            AddNextPlot("Evolution steps", _plotWidth, _plotHeight, _yAxisMin, _yAxisMax, _xAxisMin, _xAxisMax);
            AddPoints(positivePoints, OxyColors.Green);
            AddPoints(negativePoints, OxyColors.DarkRed);

            var stepIncrement = evolutionSteps.Count / numberOfSteps;
            var j = 0;

            for (var i = 0; i < numberOfSteps * stepIncrement; i += stepIncrement)
            {
                if (i > evolutionSteps.Count) break;

                var color = OxyColor.FromRgb(50, 50, (byte)(byte.MaxValue / numberOfSteps * j++));
                AddConstraints(evolutionSteps[i], null, color);
            }

            return this;
        }

        public Visualization PreparePlots(IList<Point> positivePoints, IList<Point> negativePoints, IList<Point> testPoints, MathModel mathModel)
        {
            PreparePlots(positivePoints, negativePoints, mathModel);
            PreparePlots(testPoints, mathModel);

            return this;
        }
      
        public Visualization PreparePlots(IList<Point> positivePoints, IList<Point> negativePoints, IList<Point> testPoints, MathModel mathModel, IList<IList<Constraint>> evolutionSteps, int numberOfSteps)
        {
            PreparePlots(positivePoints, negativePoints, mathModel, evolutionSteps, numberOfSteps);
            PreparePlots(testPoints, mathModel);

            return this;
        }
    }
}
