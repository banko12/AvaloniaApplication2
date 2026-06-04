using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace BH.Experimental.WebSerial;

public static partial class WebSerialInterop
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(WebSerialInterop))]
    static WebSerialInterop()
    {
        
    }

    // 1. Tell .NET to dynamically fetch and cache your separate JS module file
    public static Task InitializeAsync()
    {
        return JSHost.ImportAsync("WebSerialModule", "/webSerial.js");
    }

    // 2. Import methods mapping strictly to the "WebSerialModule" identifier
    [JSImport("openSerialPort", "WebSerialModule")]
    public static partial Task<bool> OpenSerialPortAsync(int baudRate);

    [JSImport("writeSerialData", "WebSerialModule")]
    public static partial Task WriteSerialDataAsync(string data);

    [JSImport("startReadLoop", "WebSerialModule")]
    public static partial Task StartReadLoopAsync();

    [JSImport("closeSerialPort", "WebSerialModule")]
    public static partial Task CloseSerialPortAsync();

    // 3. Export a C# listener method so JavaScript can return data to Avalonia
    [JSExport]
    public static void ReceiveBytes(byte[] data)
    {
        string textChunk = System.Text.Encoding.UTF8.GetString(data);

        // Broadcast this string to your Avalonia ViewModels / UI
        OnDataReceived?.Invoke(textChunk);
    }

    public static event Action<string>? OnDataReceived;
}
