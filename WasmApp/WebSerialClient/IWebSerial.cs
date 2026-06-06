using System;
using System.Threading.Tasks;

namespace AvaloniaApplication2;

public interface IWebSerial
{
    bool IsSupported { get; }

    ValueTask InitializeAsync();
    Task<bool> OpenAsync(int baudRate);
    Task CloseAsync();
    /// <summary>
    /// The sending: write bytes
    /// </summary>
    Task WriteAsync(byte[] buffer);

    //the receiving: an event that fires when data is received from the serial port.
    event Action<byte[]>? DataReceived;

}
