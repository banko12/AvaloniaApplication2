
using Avalonia.Controls;
using B;

using B.Ux;
using B.NA.App.Facade;


using static B.ShortColours;
using Avalonia.Layout;

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

        Colour bdc = clear; // Colour.Gui1.Fade(0.5);


        StackPanel buttonGroup(string name)
        {
            var bd = new Border()
            {
                BorderBrush = bdc.Brush(),
                CornerRadius = new Avalonia.CornerRadius(5),
                BorderThickness = new Avalonia.Thickness(0.7),
            }
            .Margin(0).GapLeft(5).GapRight(5)
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



        btn("Scan").AddTo(sp).WithClickEx(async () =>
        {
            await Sequences.RunCommands("AT+GAPSCAN=30");
        });

        btn("Stop").WithTooltip("sends Ctrl+C").AddTo(sp).WithClickEx(async () =>
        {
           // "send Ctrl-C".Log();
            await Sequences.SenCtrlC();
        });



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
            await Sequences.RunCommands("AT+SECLVL=1");
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
