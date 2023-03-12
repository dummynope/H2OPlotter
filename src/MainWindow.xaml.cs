using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ScottPlot;
using ScottPlot.Plottable;

namespace H2MassPercentPlotter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly H2Calc h2Calc;
    public MainWindow()
    {
        InitializeComponent();
        h2Calc = new H2Calc();
        DataContext = h2Calc;
    }

    private ScatterPlot? _sp;
    private Crosshair? _ch;

    static string CustomFormatter(double position)
    {
        if (position == 0)
            return "zero";
        if (position > 0)
            return $"+{position:N4}";
        return $"({position:N4})";
    }
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            logBox.Text = string.Empty;
            WpfPlot1.Plot.Clear();
            _ch = null;
            _sp = null;
            var res = h2Calc.Calc().ToList();
            if (!res.Any())
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in res)
            {
                sb.AppendLine(item.log);
            }

            logBox.Text = sb.ToString();

            double[] dataX = Enumerable.Range(1, res.Count).Select(o => (double)o).ToArray();// res.Select(o => o.X).ToArray();
            double[] dataY = res.Select(o => o.capacity_H2_des).ToArray();

            if (dataY.Any(o => double.IsNaN(o) || double.IsNegativeInfinity(o) || double.IsPositiveInfinity(o)) ||
                dataX.Any(o => double.IsNaN(o) || double.IsNegativeInfinity(o) || double.IsPositiveInfinity(o)))
            {
                int index = 0;
                MessageBox.Show($"plot not possible, results per line:\n{string.Join(",\n", res.Select(r => $"[{index}] = {index + 1} {r.desorped_H2}"))}", "Invalid Results");
                return;
            }

            var plt = WpfPlot1.Plot;

            _sp = WpfPlot1.Plot.AddScatter(dataX, dataY);

            if (Crosshair.IsChecked == true)
            {
                _ch = plt.AddCrosshair(dataX[0], dataY[0]);

                // use the custom formatter for X and Y crosshair labels
                _ch.HorizontalLine.PositionFormatter = CustomFormatter;
                _ch.VerticalLine.PositionFormatter = CustomFormatter;
            }

            if (Labels.IsChecked == true)
            {
                for (var index = 0; index < res.Count; index++)
                {
                    var item = res[index];
                    var marker = plt.AddMarker(index + 1, item.desorped_H2);
                    marker.Text = $"{index}";
                    marker.TextFont.Color = Color.Red;
                    marker.TextFont.Alignment = Alignment.UpperCenter;
                    marker.TextFont.Size = 28;
                }
            }


            // style the plot
            plt.Title("Hydrogen capacity MgH2+5 wt.-%Fe");
            plt.XLabel("Iteration");
            plt.YLabel("H2 capacity wt.-% (Dehydr.)");

            WpfPlot1.Refresh();
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }

    }

    private void WpfPlot1_OnMouseMove(object sender, MouseEventArgs e)
    {
        try
        {
            if (_sp == null)
                return;

            if (_ch == null)
                return;

            (double mouseCoordX, double mouseCoordY) = WpfPlot1.GetMouseCoordinates();
            double xyRatio = WpfPlot1.Plot.XAxis.Dims.PxPerUnit / WpfPlot1.Plot.YAxis.Dims.PxPerUnit;
            (double pointX, double pointY, int _/*pointIndex*/) = _sp.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
            _ch.X = pointX; _ch.Y = pointY;
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }

    }

    private void ButtonBase_Save(object sender, RoutedEventArgs e)
    {
        try
        {
            h2Calc.Save();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void ButtonBase_Load(object sender, RoutedEventArgs e)
    {
        try
        {
            h2Calc.Load();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

    }
}