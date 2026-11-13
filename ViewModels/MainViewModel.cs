using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NCalc;
using ScottPlot;
using System.Collections.ObjectModel;
using System.Globalization;


namespace MathVisualizer.ViewModels;

//Logic of the main window: reads user input, calculates points
// of the function and builds the plot

public partial class MainViewModel : ObservableObject
{
    // Size of the exported PNG image (in pixels)
    private const int ExportWidth = 800;
    private const int ExportHeight = 600;

    [ObservableProperty]
    private string _mathExpression = "2 * 3 + 4 * (4 - x)";

    // Text of the error shown under the "Calculate" button (empty = no error)
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    // Property names (Xmin, Xmax, Stepx) are generated from the fields
    // and are used in MainWindow.axaml bindings
    [ObservableProperty]
    private string _xmin = "2.0";

    [ObservableProperty]
    private string _xmax = "6.0";

    [ObservableProperty]
    private string _stepx = "1.1";

    [ObservableProperty]
    private List<double> _xValues = new List<double>();

    [ObservableProperty]
    private List<double> _yValues = new List<double>();

    public Plot Plot { get; } = new();

    public MainViewModel()
    {
        BuildPlot();  
    }

    // Raised when the plot has changed and the control must be redrawn
    public event Action? PlotUpdated;
    
    [RelayCommand]
    private void UpdateGraph()
    {
        IsFileSaved = false;
        BuildPlot();
        PlotUpdated?.Invoke();
    }   

    // History of successfully plotted expressions (newest first)
    public ObservableCollection<string> ExpressionLog { get; } = [];

    // Controls the visibility of the "New file is saved!" message
    [ObservableProperty]    
    private bool _isFileSaved; 

    // Saves the current plot to a PNG file.
    // The file dialog is opened in the View (MainWindow.axaml.cs).
    public void SavePlot(string path)
    {
        try
        {
            Plot.SavePng(path, ExportWidth, ExportHeight);
            ErrorMessage = string.Empty;
            IsFileSaved = true;
        }
        catch (Exception)
        {
            IsFileSaved = false;
            ErrorMessage = "Save Error!";
        }
    }

    // Validates the input, calculates y = f(x) for every x
    // from xmin to xmax and draws the result on the plot.
    private void BuildPlot()
    {
        // Start from an empty plot and no error.
        // Because of this, every "return" below leaves the plot empty.
        Plot.Clear();
        ErrorMessage = string.Empty;
        XValues.Clear();
        YValues.Clear();

        if (string.IsNullOrWhiteSpace(MathExpression))
        {
            ErrorMessage = "Enter an equation";
            return;
        }

        if (!double.TryParse(Xmin.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double xmin))
        {
            ErrorMessage = "Error in entering xmin value";
            return;
        }

        if (!double.TryParse(Xmax.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double xmax))
        {
            ErrorMessage = "Error in entering xmax value";
            return;
        }

        if (!double.TryParse(Stepx.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double stepx))
        {
            ErrorMessage = "Error in entering step x value";
            return;
        }

        if (xmin >= xmax)
        {
            ErrorMessage = "Error! xmin >= xmax";
            return;
        }

        if (stepx > (xmax - xmin))
        {
            ErrorMessage = "Error! Step x is bigger than difference between xmin and xmax";
            return;
        }
        // Without this check the loop would never end (step 0 or negative)
        if (stepx <= 0) {
            ErrorMessage = "Step x must be greater than 0"; 
            return; 
        }

        // Protection from too many points (a tiny step would freeze the app)
        if ((xmax - xmin) / stepx > 100_000)
        {
            ErrorMessage = "Error! Too many points, increase step x";
            return;
        }

        try
        {
            var expression = new Expression(MathExpression, ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
            expression.Parameters["pi"] = Math.PI;
            expression.Parameters["e"] = Math.E;

            if (expression.HasErrors())
            {
                ErrorMessage = $"Syntax Error: {expression.Error}";
                return;
            }

            for (double x = xmin; x <= xmax; x+= stepx)
            {
                expression.Parameters["x"] = x;
                var result = expression.Evaluate();

                if (result != null && double.TryParse( Convert.ToString(result, CultureInfo.InvariantCulture),
                NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
                {
                    if (double.IsNaN(y) || double.IsInfinity(y)) 
                        continue;
                    
                    XValues.Add(x);
                    YValues.Add(y);
                }

            }

            Plot.Add.ScatterLine(XValues.ToArray(), YValues.ToArray());
            Plot.Axes.AutoScale();
            
            string trimmedExp = MathExpression.Trim();
            ExpressionLog.Remove(trimmedExp);
            ExpressionLog.Insert(0, trimmedExp);
        }

        catch (Exception)
        {
            ErrorMessage = "Calculation Error!";
            Plot.Clear();
            return;
        }
    }
}


