using AvaloniaApplication2;
using System;
using System.Threading.Tasks;

namespace BH.Experimental.WebSerial;

internal sealed class BrowserWebSerial : IWebSerial
{
    private bool _initialized;

    public bool IsSupported => true;

    public event Action<string>? DataReceived;

    public BrowserWebSerial()
    {
        WebSerialInterop.OnDataReceived += OnInteropDataReceived;
    }

    private void OnInteropDataReceived(string text) => DataReceived?.Invoke(text);

    private async ValueTask EnsureInitializedAsync()
    {
        if (_initialized)
            return;

        await WebSerialInterop.InitializeAsync();
        _initialized = true;
    }

    public async ValueTask InitializeAsync() => await EnsureInitializedAsync();

    public async Task<bool> OpenAsync(int baudRate)
    {
        await EnsureInitializedAsync();
        return await WebSerialInterop.OpenSerialPortAsync(baudRate);
    }

    public async Task StartReadLoopAsync()
    {
        await EnsureInitializedAsync();
        await WebSerialInterop.StartReadLoopAsync();
    }

    public async Task WriteAsync(string data)
    {
        await EnsureInitializedAsync();
        await WebSerialInterop.WriteSerialDataAsync(data);
    }

    public async Task CloseAsync()
    {
        await EnsureInitializedAsync();
        await WebSerialInterop.CloseSerialPortAsync();
    }
}
