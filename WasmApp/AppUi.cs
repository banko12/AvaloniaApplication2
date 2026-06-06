using Avalonia.Controls;
using B;
using B.NA.App.Facade;
using B.NA.Ux3;
using B.NA.Ux3.Facade;
using B.Plots;
using B.Ux;
using System.Text;
using System.Threading.Tasks;
using static B.ShortColours;
using static B.ShortMath;

namespace AvaloniaApplication2;

static class AppUi
{
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

        Ui.LabelCenter("BleuIO Serial").GapTop(20).AddTo(sp);

        WebSerial.Current.DataReceived += bytes =>
        {
            string textChunk = Encoding.UTF8.GetString(bytes);
            $"{textChunk}".Log();
        };

        ui.Btn("Open port").AddTo(sp).WithClickEx(async () =>
        {
            var x = await WebSerial.Current.OpenAsync(115200);

            if (x)
            {
                "Port opened successfully".Log(green);
            }
            else
            {
                "Failed to open port".Log(red);
                return;
            }
        });

        ui.Btn("Close port").AddTo(sp).WithClickEx(async () =>
        {
            await WebSerial.Current.CloseAsync();
            "Port closed".Log(gray);
        });

        ui.LineEntry().AddTo(sp).WithAction(async x =>
        {
            var y = x.TrimEnd('\n', ' ') + "\r\n";
            var bytes = Encoding.UTF8.GetBytes(y);
            await WebSerial.Current.WriteAsync(bytes);
        });

        ui.Btn("Send ATI").AddTo(sp).WithClickEx(async () =>
        {
            var bytes = Encoding.UTF8.GetBytes("ATI\r\n");
            await WebSerial.Current.WriteAsync(bytes);
        });
    }
}
