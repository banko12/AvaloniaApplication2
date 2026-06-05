using System;
using System.Threading.Tasks;

namespace AvaloniaApplication2;

public interface IWebSerial
{
    bool IsSupported { get; }
    event Action<string>? DataReceived;

    ValueTask InitializeAsync();
    Task<bool> OpenAsync(int baudRate);
    Task StartReadLoopAsync();
    Task WriteBufferAsync(byte[] buffer);
    Task CloseAsync();
}
