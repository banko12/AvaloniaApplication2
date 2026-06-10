using Avalonia.Controls;
using B;
using B.NA.App.Facade;
using B.NA.Ux3.Facade;
using B.Ux;
using BH.BleuIO;
using BtkApp;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using static B.ShortColours;

namespace AvaloniaApplication2;

static class AppUi
{

    static PanelDongleTerminal panelDongleTerminal;

    static LogFunnel LogFunnel;

    static BleuPort port;

    private static void TLog(this string x) => LogFunnel.Add(x);

    private static string TLogComment(this string x)
    {
        const string prefix = "// ";
        string commented = prefix + x.ReplaceLineEndings(Environment.NewLine + prefix);
        commented.TLog();
        return x;
    }

    public static async Task Build(ContentControl cc)
    {
        cc.WithDisposeManager(out var dm);
        var dp = new DockPanel { Background = white.Brush() }.PlaceInside(cc);

        var statusPanelPlaceholder = new ContentControl().AddBottom(dp);

        var sp = uin.StackPanel(width: 180).AddRight(dp);

        port = new BleuPort().DisposedBy(dm);

        Sequences.SetApi(port);
        LogFunnel = new LogFunnel().DisposedBy(dm);

        panelDongleTerminal = new PanelDongleTerminal().AddTo(dp);

        LogFunnel.Observable
            .ObserveOnUi()
            .Subscribe(x => panelDongleTerminal.Terminal.Log(x))
            .DisposedBy(dm);

        //hook up the dongle traffic to be rendered in the Dongle Terminal
        port.IncomingLines.Merge(port.Outgoing)
            .Subscribe(line => line.TLog())
            .DisposedBy(dm);

        panelDongleTerminal.Terminal.Lines
            .Subscribe(async cmd =>
            {
                await port.Send(cmd);
            })
            .DisposedBy(dm);

        var sp1 = Ui.StackPanel().PlaceInside(new Header("Port").GapTop(-10).AddTo(sp));
        ui.Btn("Open").AddTo(sp1).WithClickEx(async () =>
        {

            var x = await port.Open();

            if (!x)
            {
                "Error: failed to open port".TLogComment();
                return;
            }

            try
            {
                "Port opened successfully".TLogComment();
                await Init(port);

            }
            catch (Exception ex)
            {
                $"Error during port initialization: {ex.Message}".TLogComment();

            }

            panelDongleTerminal.Terminal.ScheduleFocus(afterMs: 100);

        });

        ui.Btn("Close").AddTo(sp1).WithClickEx(async () =>
        {
            await port.Close();
            "Port closed".TLogComment(); 
        });

        var statusBar = new PanelStatusBar().PlaceInside(statusPanelPlaceholder);

    }

    static async Task Init(BleuPort bport)
    {
        const int dms = 200;

        string[] cmds = [
            "ATE0",
            "ATA0",
            "ATEW0",
            "ATDS0",
            "AT+CENTRAL",
            "AT+GAPIOCAP=4",
            "AT+CONNPARAM=15=15=0=1000",
            "ATI"
            ];

        try
        {
            //note: this will return quickly if we are already in verbose mode
            //if we are not, it will not recognize the plain text response and
            //will "fail" after the timeout 
            //we don't really check for the result. All we are doing is that if we are already
            //in verbose mode, we don't incur waiting extra time
            int res = 0;
            await bport.Expect("ATV1", x => ResponseParsers.IsVerbose(x, out res), dms);

            foreach (var cmd in cmds)
            {
                await bport.ExpectThrow(cmd, x => ResponseParsers.IsResult(x, out res), dms);
            }
            await Task.Delay(100);

           // Log = "// Init dongle completed";
        }
        catch (Exception ex)
        {
           // Log = $"-r {ex.Message}";
        }
    }

}
