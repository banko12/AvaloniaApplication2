using Avalonia.Controls;
using B;
using B.NA.App.Facade;
using B.NA.Ux3;
using B.NA.Ux3.Facade;
using B.Plots;
using B.Ux;
using System.Threading.Tasks;
using static B.ShortColours;
using static B.ShortMath;

namespace AvaloniaApplication2;

static class AppUi
{
    public const string AppName = "HidTerm";
    public static async Task Build(ContentControl cc)
    {
        cc.WithDisposeManager(out var dm);
        var dp = new DockPanel { Background = white.Brush() }.PlaceInside(cc);

        var sp = uin.StackPanel(width: 180).AddRight(dp);
        ui.LoggerWithCopyAndClear().SetGlobalLogger().AddTo(dp);

        int k = 0;
        ui.Btn("Run").AddTo(sp).WithClickEx(async () =>
        {
            $"hello {k++}".Log(blue);

            $"hello copytext {k++}".LogCopytext("copy text");
        });

        ui.Btn("Quick").AddTo(sp).WithClick(() =>
        {
            linspace(0, pi2).Apply(sin).Plot().Log();
        });

        ui.Btn("Multiple lines").AddTo(sp).WithClick(() =>
        {
            linspace(0, pi2).Apply(sin).Plot(purple).Ref(out var p).Log();
            linspace(0, pi2).Apply(cos).AddLine(p, red);
            linspace(0, pi2).Apply(x => cos(3 * x)).AddScatter(p, purple);
        });


        ui.Btn("Run").AddTo(sp).WithClickEx(async () =>
        {
            var x =await WebSerial.Current.OpenAsync(9600);
            
            
            x.Log("Opened port:");
        });
    }

}
