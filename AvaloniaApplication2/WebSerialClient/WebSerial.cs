using System;
using System.Threading.Tasks;

namespace AvaloniaApplication2;

public static class WebSerial
{
    private static IWebSerial _current = new NotSupportedWebSerial();

    public static IWebSerial Current => _current;

    public static void SetCurrent(IWebSerial implementation)
        => _current = implementation ?? throw new ArgumentNullException(nameof(implementation));

    private sealed class NotSupportedWebSerial : IWebSerial
    {
        public bool IsSupported => false;
        public event Action<byte[]>? DataReceived { add { } remove { } }

        public ValueTask InitializeAsync() => ValueTask.CompletedTask;

        public Task<bool> OpenAsync(int baudRate) => Task.FromException<bool>(
            new PlatformNotSupportedException("WebSerial is only available in the Browser (WASM) build."));

        //public Task StartReadLoopAsync() => Task.FromException(
        //    new PlatformNotSupportedException("WebSerial is only available in the Browser (WASM) build."));
 
        public Task WriteAsync(byte[] buffer) => Task.FromException(
            new PlatformNotSupportedException("WebSerial is only available in the Browser (WASM) build."));

        public Task CloseAsync() => Task.FromException(
            new PlatformNotSupportedException("WebSerial is only available in the Browser (WASM) build."));
    }
}