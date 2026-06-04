using Avalonia;
using Avalonia.Browser;
using AvaloniaApplication2;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args) => 
        BuildAvaloniaApp()
        .StartBrowserAppAsync("out");  //the div id in index.html to mount the app to

    public static AppBuilder BuildAvaloniaApp()
    { 
        var builder =  AppBuilder.Configure<App>()
                        .WithInterFont()
                        #if DEBUG
                        .WithDeveloperTools()
                        #endif
                        ;

        return builder;
    }
}
