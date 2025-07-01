using Avalonia.Controls;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ServoGapApp;

public partial class MainWindow : Window
{
    private MainWindowViewModel _viewModel;

    public MainWindow()
    {
        DataContext = _viewModel = new();

        InitializeComponent();
        this.ExtendClientAreaToDecorationsHint = true;
        this.ExtendClientAreaTitleBarHeightHint = 30d;
        this.Activate();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}