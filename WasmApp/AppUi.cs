using Avalonia.Controls;
using B;
using B.NA.App.Facade;
using B.NA.Ux3;
using B.NA.Ux3.Facade;
//using B.Plots;
using B.Ux;
using BtkApp;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using static B.ShortColours;
//sing static B.ShortMath;

namespace AvaloniaApplication2;

static class AppUi
{

    static PanelDongleTerminal panelDongleTerminal;

    static LogFunnel LogFunnel;

    static BP port;

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

        var sp = uin.StackPanel(width: 180).AddRight(dp);
        ui.LoggerWithCopyAndClear().Width(300).SetGlobalLogger().AddRight(dp);


        port = new BP().DisposedBy(dm);

        LogFunnel = new LogFunnel().DisposedBy(dm);

        panelDongleTerminal = new PanelDongleTerminal().AddTo(dp);
        panelDongleTerminal.Terminal.Log(Data.Sample);


        LogFunnel.Observable
            .ObserveOnUi()
            .Subscribe(x => panelDongleTerminal.Terminal.Log(x))
            .DisposedBy(dm);


       


    //var h = new Header("Terminal").WithShadow(0.15, 10).AddTo(dp);

    //var term = new Terminal().Margin(5).PlaceInside(h)
    //                .WithHighlighter(Highlighter.Get())
    //                .ScheduleFocus();




    //int k = 0;
    //    ui.Btn("Run").AddTo(sp).WithClickEx(async () =>
    //    {
    //        $"hello {k++}".Log(blue);
    //        $"hello copytext {k++}".LogCopytext("copy text");
    //    });

    //    ui.Btn("Quick").AddTo(sp).WithClick(() =>
    //    {
    //        linspace(0, pi2).Apply(sin).Plot().Log();
    //    });

    //    ui.Btn("Multiple lines").AddTo(sp).WithClick(() =>
    //    {
    //        linspace(0, pi2).Apply(sin).Plot(purple).Ref(out var p).Log();
    //        linspace(0, pi2).Apply(cos).AddLine(p, red);
    //        linspace(0, pi2).Apply(x => cos(3 * x)).AddScatter(p, purple);
    //    });

      //  Ui.LabelCenter("BleuIO Serial").GapTop(20).AddTo(sp);

        //WebSerial.Current.DataReceived += bytes =>
        //{
        //    string textChunk = Encoding.ASCII.GetString(bytes);
        //    $"{textChunk}".Log();


        //    textChunk.TLog();
        //};

        ui.Btn("Open port").AddTo(sp).WithClickEx(async () =>
        {

            var x = await port.Open();

          //  var x = await WebSerial.Current.OpenAsync(115200);

            if (x)
            {
                "Port opened successfully".TLogComment(); //y.Log(green);
            }
            else
            {
                "Failed to open port".TLogComment(); //.Log(red);
                return;
            }
        });

        ui.Btn("Close port").AddTo(sp).WithClickEx(async () =>
        {
          //  await WebSerial.Current.CloseAsync();

            await port.Close();
            "Port closed".TLogComment(); //.Log(gray);
        });


        //static async Task Send(string cmd)
        //{
        //    var y = cmd.TrimEnd('\n', ' ') + "\r\n";
        //    var bytes = Encoding.ASCII.GetBytes(y);
        //    await WebSerial.Current.WriteAsync(bytes);



        //}


        //ui.LineEntry().AddTo(sp).WithAction(async x =>
        //{
        //    await port.Send(x);
        //});

        ui.Btn("Send ATI").AddTo(sp).WithClickEx(async () =>
        {
            await port.Send("ATI");
        });


        //hook up the dongle traffic to be rendered in the Dongle Terminal
        port.IncomingLines.Merge(port.Outgoing)
           // .WithGate(out var gate, enabled: true)

            // .ObserveOnDispatcher()
            .Subscribe(line => line.TLog())
            .DisposedBy(dm);



        panelDongleTerminal.Terminal.Lines
            .Subscribe(async cmd => 
            {
                await port.Send(cmd);
               // cmd.TLog();
              //  await Send(cmd);
            })
            .DisposedBy(dm);
    }
}
