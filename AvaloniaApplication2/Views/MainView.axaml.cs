using Avalonia.Controls;


using B;

using B.NA.App.Facade;
using B.NA.Ux3.Facade;
using B.Ux;



namespace AvaloniaApplication2.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        var dp = new DockPanel();
        this.Content = dp;

        var sp = uin.StackPanel(width: 180).AddRight(dp);
        ui.LoggerWithCopyAndClear().SetGlobalLogger().AddTo(dp);
        int i = 0;
        ui.Btn("Run").AddTo(sp).WithClickEx(async () =>
        {
            $"hello {i++}".Log();
        });

    }
}