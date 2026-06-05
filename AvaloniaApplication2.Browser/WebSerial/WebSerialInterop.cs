using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace BH.Experimental.WebSerial;
/// <summary>
/// This class works together with the "webSerial.js" module
/// to create a C# interop layer for the Web Serial API 
/// </summary>
public static partial class WebSerialInterop
{
    // 1. Tell .NET to dynamically fetch and cache your separate JS module file
    public static Task InitializeAsync()
    {
        // dotnet.js lives in ".../<app>/_framework/..."
        // so "../webSerial.js" resolves to ".../<app>/webSerial.js" (works on GitHub Pages subpath too)
        return JSHost.ImportAsync("WebSerialModule", "../webSerial.js");
    }

    // 2. Import methods mapping strictly to the "WebSerialModule" identifier
    [JSImport("openSerialPort", "WebSerialModule")]
    public static partial Task<bool> OpenSerialPortAsync(int baudRate);


    [JSImport("writeBuffer", "WebSerialModule")]
    public static partial Task WriteBufferAsync(byte[] buffer);


    [JSImport("startReadLoop", "WebSerialModule")]
    public static partial Task StartReadLoopAsync();

    [JSImport("closeSerialPort", "WebSerialModule")]
    public static partial Task CloseSerialPortAsync();

    // 3. Export a C# listener method so JavaScript can return data to Avalonia
    [JSExport]
    public static void ReceiveBytes(byte[] data)
    {
       // string textChunk = System.Text.Encoding.UTF8.GetString(data);
       // OnDataReceived?.Invoke(textChunk);

        OnDataReceived?.Invoke(data);
    }

    public static event Action<byte[]>? OnDataReceived;
}
