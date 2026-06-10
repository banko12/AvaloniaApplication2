using B;

using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BtkApp;

/// <summary>
/// The idea of this is to have a funnel where log messages can be sent from multiple threads,
/// preserving as much as possible the order of arrival, and then emitted on a single thread.
/// Additionally, it has an enable/disable mechanism that can optionally stop logging.
/// For example, when there is fast traffic that the UI cannot keep up with, such as during streaming,
/// logging can be disabled temporarily.
/// </summary>
public sealed class LogFunnel : IDisposable
{
    readonly DisposeManager dm = new();
    public void Dispose() => dm.Dispose();

    readonly Subject<string> subj;
    public IObservable<string> Observable => subj;

    readonly Channel<string> channel;

    int isEnabled = 1;

    public LogFunnel(int bufferCapacity = 100)
    {
        subj = new Subject<string>().DisposedBy(dm);
        channel = Channel.CreateBounded<string>(bufferCapacity);
        dm.Push(Stop);
        Task.Run(async () => ReadTask());
    }

    public void Enable(bool enable) => Interlocked.Exchange(ref isEnabled, enable ? 1 : 0);

    void Stop()
    {
        Enable(false);
        channel.Writer.TryComplete();
    }

    async void ReadTask()
    {
        await foreach (var item in channel.Reader.ReadAllAsync())
        {
            subj.OnNext(item);
        }
    }

    public void Add(string message)
    {
        if (Interlocked.CompareExchange(ref isEnabled, 1, 1) == 0) return;
        channel.Writer.TryWrite(message);
    }
}