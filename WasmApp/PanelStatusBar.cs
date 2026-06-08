
using Avalonia.Controls;
using Avalonia.Layout;
using B;
using B.NA.App.Facade;
using B.Ux;
using static B.ShortColours;

namespace BtkApp;

class PanelStatusBar : UiGroup<DockPanel>
{
    public PanelStatusBar()
    {
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
            .AddLeft(root);
            var sp = new StackPanel() { Orientation = Orientation.Horizontal }.PlaceInside(bd);
            uin.LabelLeft(name).GapRight(5).GapLeft(5).AddTo(sp);
            return sp;
        }

        var sp = buttonGroup("Scan");

        btn("Scan").AddTo(sp).WithClickEx(async () =>
        {
            await Sequences.RunCommands("AT+GAPSCAN=30");
        });

        btn("Stop").AddTo(sp).WithClickEx(Sequences.SenCtrlC);

        sp = buttonGroup("Security");

        btn("Get").AddTo(sp).WithClickEx(async () =>
        {
            await Sequences.RunCommands("AT+SECLVL");
        });

        btn("Set 1").AddTo(sp).WithClickEx(async () =>
        {
            await Sequences.RunCommands("AT+SECLVL=1");
        });

        btn("Set 4").AddTo(sp).WithClickEx(async () =>
        {
            await Sequences.RunCommands("AT+SECLVL=4");
        });

        sp = buttonGroup("Info");

        btn("ATI").AddTo(sp).WithClickEx(async () =>
        {
            await Sequences.RunCommands("ATI");
        });

        btn("GETCONN").AddTo(sp).WithClickEx(async () =>
        {
            await Sequences.RunCommands("AT+GETCONN");
        });

        btn("GETBOND").AddTo(sp).WithClickEx(async () =>
        {
            await Sequences.RunCommands("AT+GETBOND");
        });

        root.Fill();
    }
}
