using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Themes.Simple;
using AvaloniaApplication2.ViewModels;
using AvaloniaApplication2.Views;
using B.Ux;
using System.Linq;

namespace AvaloniaApplication2;

public partial class App : Application
{
    public override void Initialize()
    {



        AvaloniaXamlLoader.Load(this);

        PropsAvalonia.Init();
        var theme = new SimpleTheme();
        this.Styles.Add(theme);

        var bhaTheme = new BHA.Theme();
        this.Styles.Add(bhaTheme);
        this.RequestedThemeVariant = ThemeVariant.Default;

    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory = () => new MainView { DataContext = new MainViewModel() };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}