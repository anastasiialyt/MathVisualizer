using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MathVisualizer.ViewModels;

namespace MathVisualizer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var vm = new MainViewModel();
        DataContext = vm;

        // Show the plot from the ViewModel in the control
        PlotControl.Reset(vm.Plot);

        // Redraw the control every time the ViewModel rebuilds the plot
        vm.PlotUpdated += PlotControl.Refresh;

        // Draw the initial plot
        PlotControl.Refresh();
    }

    // Handler of the "Save plot in a new file" button
    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
            return;

        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save plot",
            SuggestedFileName = "MathVisualizer_Plot.png",
            DefaultExtension = "png",
            FileTypeChoices = [FilePickerFileTypes.ImagePng]
        });

        if (file is null)
            return;

        vm.SavePlot(file.Path.LocalPath);
    }
}