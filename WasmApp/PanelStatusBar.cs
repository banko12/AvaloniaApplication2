
using Avalonia.Controls;
using B;

using B.Ux;
using B.NA.App.Facade;


using static B.ShortColours;
using Avalonia.Layout;
using System.Threading.Tasks;

namespace BtkApp;



class PanelStatusBar : UiGroup<DockPanel>
{
    public PanelStatusBar()
    {
       // var api = App.Dongle;
       

        Btn btn(string s) => ui.Btn(s).WithProps(new{
            CornerRadius=3,
            Width=70 
        });

        var bdc = clear.Brush();
        var bdb = white.Brush(); // blue.Fade(0.95).Brush();

        StackPanel buttonGroup(string name)
        {
            var bd = new Border()
            {
                Background = bdb,
                BorderBrush = bdc,
                CornerRadius = new Avalonia.CornerRadius(5),
                BorderThickness = new Avalonia.Thickness(0.7),
            }
            .WithShadow(opacity:0.15)
            .Margin(10)
            .GapTop(-5)
           // .Margin(0).GapLeft(5).GapRight(5)
            .AddLeft(root);
            var sp = new StackPanel() { Orientation = Orientation.Horizontal }.PlaceInside(bd);

            uin.LabelLeft(name).GapRight(5).GapLeft(5).AddTo(sp);

            return sp;
        }

        var sp = buttonGroup("Scan");

        //var bd = new Border() 
        //    { BorderBrush = bdc, 
        //    CornerRadius = new System.Windows.CornerRadius(4), 
        //    BorderThickness =new System.Windows.Thickness(1),
        //   // Height=40 
        //}
        //    .Margin(10)
        //    .AddLeft(root);

        //var sp = new StackPanel() {  Orientation=Orientation.Horizontal}.PlaceInside(bd);



        btn("Scan").WithTooltip("Scan for 30s").AddTo(sp).WithClickEx(async () =>
        {
            await Sequences.RunCommands("AT+GAPSCAN=30");
        });

        btn("Stop").WithTooltip("Send Ctrl+C (attempt to cancel scan)").AddTo(sp).WithClickEx(async () =>
        {
           // "send Ctrl-C".Log();
            await Sequences.SenCtrlC();
            await Task.Delay(50);
            await Sequences.SenCtrlC();
            await Task.Delay(50);
            await Sequences.SenCtrlC();
        });



        sp = buttonGroup("Security");



        btn("Get").AddTo(sp).WithTooltip("Get security level").WithClickEx(async () =>
        {
            await Sequences.RunCommands("AT+SECLVL");
        });

        btn("Set 1").AddTo(sp).WithTooltip("Set security level to 1").WithClickEx(async () =>
        {
            await Sequences.RunCommands("AT+SECLVL=1");
        });

        btn("Set 4").AddTo(sp).WithTooltip("Set security level to 4").WithClickEx(async () =>
        {
            await Sequences.RunCommands("AT+SECLVL=4");
        });

        sp = buttonGroup("Info");

        btn("ATI").AddTo(sp).WithTooltip("Get dongle info").WithClickEx(async () =>
        {
            await Sequences.RunCommands("ATI");
        });

        btn("GETCONN").AddTo(sp).WithTooltip("Get connection info").WithClickEx(async () =>
        {
            await Sequences.RunCommands("AT+GETCONN");
        });

        btn("GETBOND").AddTo(sp).WithTooltip("Get bond info").WithClickEx(async () =>
        {
            await Sequences.RunCommands("AT+GETBOND");
        });

        root.Fill();

    }
}
