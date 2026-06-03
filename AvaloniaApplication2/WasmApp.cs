using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Themes.Simple;
using B.Ux;
using System.Threading.Tasks;

namespace AvaloniaApplication2;

public abstract class WasmApp : Application
{
    public override void Initialize()
    {

        PropsAvalonia.Init();
        var theme = new SimpleTheme();
        this.Styles.Add(theme);

        var bhaTheme = new BHA.Theme();
        this.Styles.Add(bhaTheme);
        this.RequestedThemeVariant = ThemeVariant.Default;

    }

    public override void OnFrameworkInitializationCompleted()
    {

        var mv = new ContentControl();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new Window() { Content = mv };

        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            //this is the web  lifetime

            singleViewPlatform.MainView = mv;

        }

        BuildUi(mv);

        base.OnFrameworkInitializationCompleted();
    }

    public abstract Task BuildUi(ContentControl container);

}