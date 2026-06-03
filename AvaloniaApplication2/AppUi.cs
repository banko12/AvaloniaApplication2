using Avalonia.Controls;
using B;
using B.NA.App.Facade;
using B.NA.Ux3;
using B.NA.Ux3.Facade;
using B.Ux;
using System.Threading.Tasks;

using static B.ShortColours;

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
    }



}
