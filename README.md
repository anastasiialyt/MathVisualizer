# MathVisualizer

MathVisualizer is a desktop app for plotting a math expression. You enter a formula, such as `sin(x)` or `2 * x + 1`, choose the range and the step, and the app draws the graph.

It is a personal student project built with C#, .NET 10, and Avalonia.

## What the project does

The app reads four values:

- **xmin** is the start of the range.
- **xmax** is the end of the range.
- **step** is the distance between points.
- **equation** is the formula. Use `x` as the variable.

After you press **Calculate**, the app checks the input, calculates `y` for every `x`, and draws the line. Each formula that was drawn is added to the equation log. **Save plot in a new file** saves the current graph as a PNG image.

Both a dot and a comma are accepted, so `0.1` and `0,1` mean the same number.

## Why it is useful

It shows the shape of a function without writing a separate program for every formula. The checks happen before drawing, so a bad value does not freeze the window. You get a short error message instead.

The app rejects:
- an empty formula
- a value that is not a number
- a range where `xmin` is greater than or equal to `xmax`
- a step that is `0`, negative, or larger than the whole range
- a step so small that the graph would contain more than 100,000 points
- a formula with a syntax error

If a point is `NaN` or infinity, that point is skipped and the rest of the graph is still drawn.

## Getting started
Install the [.NET 10 SDK](https://dotnet.microsoft.com/download).
Open `MathVisualizer.csproj` in Visual Studio or Rider and run the project.
From the terminal, open the project folder and run:

```bash
dotnet run
```

## Example
| Field | Value |
| --- | --- |
| xmin | `2.0` |
| xmax | `6.0` |
| step | `1.1` |
| equation | `2 * 3 + 4 * (4 - x)` |

Press **Calculate**. The graph of one full sine wave appears, and `sin(x)` is added to the equation log.

More formulas you can try:
- `2 * x + 1`
- `x * x`
- `cos(x) + sin(x)`
- `sqrt(x)` with `xmin` set to `0`
- `2 * pi * x`

The formula can use `x`, `pi`, `e`, and common functions such as `sin`, `cos`, `tan`, `sqrt`, and `abs`. Spaces and letter case do not matter.

## How to save a graph
1. Build a graph with **Calculate**.
2. Press **Save plot in a new file**.
3. Choose a folder and a file name. The suggested name is `MathVisualizer_Plot.png`.
4. When the file is saved, the window shows **New file is saved!**

## Project structure
- `ViewModels/MainViewModel.cs` reads the input, checks it, calculates the points, and builds the plot.
- `Views/MainWindow.axaml` is the window layout.
- `Views/MainWindow.axaml.cs` shows the plot and opens the save dialog.
- `Program.cs` and `App.axaml` start the application.

The calculation stays in the view model. The window only displays data and opens the file dialog.

## Help
If something does not work, open an issue in this repository. Include the formula and the values of xmin, xmax, and step, plus the error text shown in the window.

## Author
This project is maintained by Anastasiia Lytvynenko.
