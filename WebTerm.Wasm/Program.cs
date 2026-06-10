using Avalonia;
using Avalonia.Browser;
using AvaloniaApplication2;
using BH.Experimental.WebSerial;
using System.Threading.Tasks;

internal sealed partial class Program
{
    static async Task Main(string[] args)
    {
        WebSerial.SetCurrent(new BrowserWebSerial());

        var builder = AppBuilder.Configure<App>()
                #if DEBUG
                .WithDeveloperTools()
                #endif
                ;


        await builder
        .StartBrowserAppAsync("out");  //the div id in index.html to mount the app to

    }
}
