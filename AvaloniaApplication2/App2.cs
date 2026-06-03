using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Themes.Simple;
using B;
using B.NA.App.Facade;
using B.NA.Ux3.Facade;
using B.Ux;
using System.Threading.Tasks;

namespace AvaloniaApplication2;

public class App2 : Application
{
    public override void Initialize()
    {



       // AvaloniaXamlLoader.Load(this);

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


    public async Task BuildUi(ContentControl container)
    {
        var dp = new DockPanel();
        container.Content = dp;

        var sp = uin.StackPanel(width: 180).AddRight(dp);
        ui.LoggerWithCopyAndClear().SetGlobalLogger().AddTo(dp);
        int i = 0;
        ui.Btn("Run").AddTo(sp).WithClickEx(async () =>
        {
            $"hello {i++}".Log();
        });
    }
}