//using Avalonia;
//using Avalonia.Controls;
//using Avalonia.Controls.ApplicationLifetimes;
//using Avalonia.Styling;
//using Avalonia.Themes.Simple;
//using B.Ux;
//using System.Threading.Tasks;

//namespace AvaloniaApplication2;

//public abstract class WasmApp : Application
//{
//    public override void Initialize()
//    {
//        PropsAvalonia.Init();
//        var theme = new SimpleTheme();
//        this.Styles.Add(theme);
//        var bhaTheme = new BHA.Theme();
//        this.Styles.Add(bhaTheme);
//        this.RequestedThemeVariant = ThemeVariant.Default;
//    }

//    public override void OnFrameworkInitializationCompleted()
//    {
//        //create a content control that will hold our UI
//        var root = new ContentControl();

//        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
//        {
//            //if we are inside a desktop lifetime, create a window and set its root to our content control
//            desktop.MainWindow = new Window() { Content = root };
//        }
//        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
//        {
//            //if we are inside a single view lifetime (WASM), set the main view to our content control
//            singleViewPlatform.MainView = root;
//        }

//        base.OnFrameworkInitializationCompleted();

//        //BuildUi asynchronous. We fire and forget it.
//        BuildUi(root);

//    }

//    /// <summary>
//    /// The abstract method that derived classes must implement to build the UI. 
//    /// </summary>
//    /// <param name="container">The ContentControl that will hold the app's UI.</param>
//    /// <returns>A Task representing the asynchronous operation.</returns>
//    public abstract Task BuildUi(ContentControl container);

//}