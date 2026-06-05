using AvaloniaApplication2;
using System;
using System.Threading.Tasks;

namespace BH.Experimental.WebSerial;

/// <summary>
/// In order to pass the WebSerial API to the app , we define the IWebSerial interface 
/// and provide this implementation, which forwards to the WebSerialInterop static class. 
/// At application start, we pass this IWebSerial implementation to the app.
/// </summary>
internal sealed class BrowserWebSerial : IWebSerial
{
    private bool initialized;

    public bool IsSupported => true;

    public event Action<string>? DataReceived;

    public BrowserWebSerial()
    {
        WebSerialInterop.OnDataReceived += OnInteropDataReceived;
    }

    void OnInteropDataReceived(string text) => DataReceived?.Invoke(text);

    async ValueTask EnsureInitializedAsync()
    {
        if (initialized) return;
        await WebSerialInterop.InitializeAsync();
        initialized = true;
    }

    public async ValueTask InitializeAsync() => await EnsureInitializedAsync();

    public async Task<bool> OpenAsync(int baudRate)
    {
        await EnsureInitializedAsync();
        var b =  await WebSerialInterop.OpenSerialPortAsync(baudRate);
        if(!b)  return false;

        //this is async, but it doesn't complete until the port is closed,
        //so it must be fire and forget
        WebSerialInterop.StartReadLoopAsync();
        return true;

    }

    //public async Task StartReadLoopAsync()
    //{
    //    await EnsureInitializedAsync();
    //    await WebSerialInterop.StartReadLoopAsync();
    //}

    public async Task WriteAsync(byte[] buffer)
    {
        await EnsureInitializedAsync();
        await WebSerialInterop.WriteBufferAsync(buffer);
    }

    public async Task CloseAsync()
    {
        await EnsureInitializedAsync();
        await WebSerialInterop.CloseSerialPortAsync();
    }
}
